using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Death / Respawn
    public GameObject deathScreen;
    public Transform respawnPoint;
    public float fadeSpeed = 2f;

    private CanvasGroup deathScreenGroup;
    private SpriteRenderer spriteRenderer;
    private bool isDead = false;

    // Movement
    public InputAction MoveAction;
    private Rigidbody2D rigidbody2d;
    private Vector2 move;

    [Header("Base Stats (no bonus)")]
    public float baseSpeed = 3.0f;
    public int baseMaxHealth = 5;
    public int baseAttackDamage = 1;

    [Header("Runtime Stats (with bonus)")]
    public float speed;          // used by movement
    public int maxHealth;        // used by health clamp
    public int attackDamage;     // used later for projectile damage

    // Health
    private int currentHealth;
    public int health { get { return currentHealth; } }

    // Invincibility
    public float timeInvincible = 2.0f;
    private bool isInvincible;
    private float damageCooldown;

    // Animation
    private Animator animator;
    private Vector2 moveDirection = new Vector2(1, 0);

    // Projectile
    public GameObject projectilePrefab;

    // Kunai ammo (Inventory item)
    [Header("Kunai Ammo")]
    public ItemData kunaiItemData;

    private PlayerProgression progression;

    void Start()
    {
        Time.timeScale = 1f; //fluidité
        
        MoveAction.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        deathScreenGroup = deathScreen.GetComponent<CanvasGroup>();

        deathScreen.SetActive(true);
        deathScreenGroup.alpha = 0;
        deathScreenGroup.interactable = false;
        deathScreenGroup.blocksRaycasts = false;

        progression = GetComponent<PlayerProgression>();

        // Init from base
        speed = baseSpeed;
        maxHealth = baseMaxHealth;
        attackDamage = baseAttackDamage;

        currentHealth = maxHealth;
        UIHandler.instance.SetHealthValue(1f);

        ApplyProgression();
    }

    void Update()
    {
        // Read movement input
        move = MoveAction.ReadValue<Vector2>();

        // Update look direction if moving
        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
        }

        // Animator params
        animator.SetFloat("Look X", moveDirection.x);
        animator.SetFloat("Look Y", moveDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        // Invincibility timer
        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0)
                isInvincible = false;
        }

        // Shoot kunai (consumes 1 from inventory)
        if (Input.GetKeyDown(KeyCode.C))
        {
            TryLaunchKunai();
        }

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    void FixedUpdate()
    {
        // speed is the CURRENT speed (base + bonus)
        Vector2 position = rigidbody2d.position + move * speed * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible) return;

            isInvincible = true;
            damageCooldown = timeInvincible;
            animator.SetTrigger("Hit");
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
    }

    void TryLaunchKunai()
    {
        if (Inventory.Instance == null) return;
        if (kunaiItemData == null) return;

        if (!Inventory.Instance.Has(kunaiItemData, 1))
            return;

        Inventory.Instance.Remove(kunaiItemData, 1);
        Launch();
    }

    void Launch()
    {
        // Sécurité: si aucune direction (ex: player immobile au lancement), on ne tire pas
        if (moveDirection.sqrMagnitude < 0.001f)
            moveDirection = Vector2.right;

        // Spawn devant le joueur (et pas toujours au-dessus)
        Vector2 spawnOffset = moveDirection.normalized * 0.6f; // ajuste 0.4f à 0.8f selon ton collider
        Vector2 spawnPos = rigidbody2d.position + spawnOffset;

        GameObject projectileObject = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        // Ignore collision joueur <-> projectile (tous les colliders si besoin)
        var playerCols = GetComponentsInChildren<Collider2D>();
        var projCols = projectileObject.GetComponentsInChildren<Collider2D>();
        foreach (var pc in playerCols)
            foreach (var prc in projCols)
                Physics2D.IgnoreCollision(prc, pc, true);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.SetDamage(attackDamage);

        // IMPORTANT: avec Impulse dans Projectile, une force 12-25 suffit souvent
        projectile.Launch(moveDirection, 20f);

        animator.SetTrigger("Launch");
    }


    void Die()
    {
        isDead = true;
        StartCoroutine(FadeInDeathScreen());
        Time.timeScale = 0.2f;

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;
    }

    //écran de mort
    IEnumerator FadeInDeathScreen()
    {
        while (deathScreenGroup.alpha < 1)
        {
            deathScreenGroup.alpha += Time.unscaledDeltaTime * fadeSpeed;
            yield return null;
        }

        deathScreenGroup.interactable = true;
        deathScreenGroup.blocksRaycasts = true;
        Time.timeScale = 0f;
    }

    //réaparission après la mort
    public void Respawn()
    {
        isDead = false;
        if (progression != null)
        {
            progression.ResetAll();
            ApplyProgression(); // remet maxHealth/speed/attack aux bases
        }


        // Re-init from current maxHealth
        currentHealth = maxHealth;

        Time.timeScale = 1f;

        if (respawnPoint != null)
            transform.position = respawnPoint.position;

        deathScreenGroup.alpha = 0;
        deathScreenGroup.interactable = false;
        deathScreenGroup.blocksRaycasts = false;

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
    }

    public void ApplyProgression()
    {
        if (progression == null) return;

        // Keep current HP percentage when maxHealth changes
        float hpPercent = (maxHealth > 0) ? (currentHealth / (float)maxHealth) : 1f;

        // Apply bonuses
        maxHealth = baseMaxHealth + progression.GetExtraMaxHealth();
        speed = baseSpeed + progression.GetExtraSpeed();
        attackDamage = baseAttackDamage + progression.GetExtraAttack();

        if (maxHealth < 1) maxHealth = 1;

        // Re-apply current health with same % (or clamp)
        currentHealth = Mathf.Clamp(Mathf.RoundToInt(hpPercent * maxHealth), 0, maxHealth);

        // Update UI
        UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
    }
}
