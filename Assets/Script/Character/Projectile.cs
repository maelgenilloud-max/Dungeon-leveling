using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Movement / Lifetime")]
    [SerializeField] private float destroyDistance = 100f;   // comme le tuto (évite qu'un projectile vive à l'infini)
    [SerializeField] private float spriteAngleOffset = -90f; // ajuste selon l'orientation du sprite
    [SerializeField] private int damage = 1;


    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 1) Destruction loin de la map (version tuto)
        if (transform.position.magnitude > destroyDistance)
        {
            Destroy(gameObject);
            return;
        }

        // 2) Orientation du kunai dans la direction du mouvement (ton script)
        // Unity 6: rb.linearVelocity (si tu es en Unity 2022, remplace par rb.velocity)
        Vector2 v = rb.linearVelocity;
        if (v.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle + spriteAngleOffset);
        }
    }

    public void Launch(Vector2 direction, float force)
    {
        rb.AddForce(direction * force);
    }

    public void SetDamage(int value)
    {
        damage = Mathf.Max(1, value);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        Destroy(gameObject);
    }

}
