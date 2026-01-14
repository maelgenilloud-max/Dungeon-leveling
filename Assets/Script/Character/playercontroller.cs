using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    // Movement
    // Death / Respawn
    public GameObject deathScreen;
    public Transform respawnPoint;
    public float fadeSpeed = 2f;

    private CanvasGroup deathScreenGroup;
    private SpriteRenderer spriteRenderer;
    private bool isDead = false;

    // Variables related to player character movement

    public InputAction MoveAction;
    private Rigidbody2D rigidbody2d;
    private Vector2 move;
    public float speed = 3.0f;

    // Health
    public int maxHealth = 5;
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
    public ItemData kunaiItemData; // drag your Kunai ScriptableObject here

    void Start()
    {
        MoveAction.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        deathScreenGroup = deathScreen.GetComponent<CanvasGroup>();

        deathScreen.SetActive(true);
        deathScreenGroup.alpha = 0;
        deathScreenGroup.interactable = false;
        deathScreenGroup.blocksRaycasts = false;

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

        // no ammo -> no shot
        if (!Inventory.Instance.Has(kunaiItemData, 1))
            return;

        // consume 1
        Inventory.Instance.Remove(kunaiItemData, 1);

        // launch projectile
        Launch();
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(
            projectilePrefab,
            rigidbody2d.position + Vector2.up * 0.5f, //spawn devant le joueur
            Quaternion.identity
        );

        // Ignore collision joueur <-> projectile
        Collider2D playerCol = GetComponent<Collider2D>();
        Collider2D projCol = projectileObject.GetComponent<Collider2D>();
        if (playerCol != null && projCol != null)
            Physics2D.IgnoreCollision(projCol, playerCol);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(moveDirection, 300);

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
        currentHealth = maxHealth;
        Time.timeScale = 1f;

        if (respawnPoint != null)
            transform.position = respawnPoint.position;

        deathScreenGroup.alpha = 0;
        deathScreenGroup.interactable = false;
        deathScreenGroup.blocksRaycasts = false;

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        UIHandler.instance.SetHealthValue(1f);
    }


}
