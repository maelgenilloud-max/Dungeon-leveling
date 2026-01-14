using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // ---------------------------
    // Patrouille (comme avant)
    // ---------------------------
    public float speed = 2f;
    public bool vertical = true;
    public float changeTime = 3.0f;

    // ---------------------------
    // Aggro / Chase
    // ---------------------------
    [Header("Chase")]
    public float chaseRadius = 3.5f;      // rayon à partir duquel il te poursuit
    public float chaseSpeed = 2.5f;       // vitesse en poursuite (souvent un peu +)
    public float stopChaseRadius = 4.5f;  // rayon pour “désaggro” (hystérésis)

    Rigidbody2D rigidbody2d;
    Animator animator;

    float timer;
    int direction = 1;

    Transform player;
    bool chasing = false;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        timer = changeTime;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // Entrée en chase
        if (!chasing && dist <= chaseRadius)
            chasing = true;

        // Sortie de chase (un peu plus loin pour éviter que ça clignote)
        if (chasing && dist >= stopChaseRadius)
            chasing = false;

        // Timer de patrouille seulement si pas en chase
        if (!chasing)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                direction = -direction;
                timer = changeTime;
            }
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector2 position = rigidbody2d.position;

        if (chasing)
        {
            Vector2 toPlayer = ((Vector2)player.position - position).normalized;
            Vector2 newPos = position + toPlayer * chaseSpeed * Time.fixedDeltaTime;
            rigidbody2d.MovePosition(newPos);

            // Anim (si tu utilises Move X / Move Y)
            if (animator != null)
            {
                animator.SetFloat("Move X", toPlayer.x);
                animator.SetFloat("Move Y", toPlayer.y);
            }
        }
        else
        {
            // Patrouille originale
            if (vertical)
            {
                position.y = position.y + speed * direction * Time.fixedDeltaTime;
                if (animator != null)
                {
                    animator.SetFloat("Move X", 0);
                    animator.SetFloat("Move Y", direction);
                }
            }
            else
            {
                position.x = position.x + speed * direction * Time.fixedDeltaTime;
                if (animator != null)
                {
                    animator.SetFloat("Move X", direction);
                    animator.SetFloat("Move Y", 0);
                }
            }

            rigidbody2d.MovePosition(position);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.ChangeHealth(-1);
        }
    }
}
