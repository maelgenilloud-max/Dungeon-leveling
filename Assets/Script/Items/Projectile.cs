using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Movement / Lifetime")]
    [SerializeField] private float destroyDistance = 100f;   // distance max parcourue depuis le tir
    [SerializeField] private float spriteAngleOffset = -90f;
    [SerializeField] private int damage = 1;

    private Rigidbody2D rb;
    private Vector2 spawnPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spawnPos = transform.position;
    }

    void Update()
    {
        // IMPORTANT: distance depuis le spawn (pas depuis 0,0)
        if (Vector2.Distance(spawnPos, rb.position) > destroyDistance)
        {
            Destroy(gameObject);
            return;
        }

        // Unity 2022: rb.velocity (pas linearVelocity)
        Vector2 v = rb.linearVelocity;
        if (v.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle + spriteAngleOffset);
        }
    }

    public void Launch(Vector2 direction, float force)
    {
        // Reset au moment exact du tir
        spawnPos = transform.position;

        // Tir propre
        rb.linearVelocity = Vector2.zero;

        // Si tu veux garder AddForce normal, garde ForceMode2D.Force.
        // Pour un projectile, Impulse est souvent mieux.
        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
    }

    public void SetDamage(int value)
    {
        damage = Mathf.Max(1, value);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Projectile touched: " + other.name);

        // NE PAS détruire sur le joueur / sol / pickup / etc.
        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
        if (enemy == null) return;

        enemy.TakeDamage(damage);
        Destroy(gameObject);
    }
}
