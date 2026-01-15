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
        GameObject projectileObject = Instantiate(
            projectilePrefab,
            rigidbody2d.position + Vector2.up * 0.5f,
            Quaternion.identity
        );

        // Ignore collision player <-> projectile
        Collider2D playerCol = GetComponent<Collider2D>();
        Collider2D projCol = projectileObject.GetComponent<Collider2D>();
        if (playerCol != null && projCol != null)
            Physics2D.IgnoreCollision(projCol, playerCol);

        Projectile projectile = projectileObject.GetComponent<Projectile>();

        // OPTIONAL (next step): pass damage to projectile if your Projectile has a damage field
        // projectile.damage = attackDamage;

        projectile.Launch(moveDirection, 300);
        projectile.SetDamage(attackDamage);


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
