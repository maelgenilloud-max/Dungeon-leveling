using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // ---------------------------
    // Patrouille
    // ---------------------------
    public float speed = 2f;
    public bool vertical = true;
    public float changeTime = 3.0f;

    // ---------------------------
    // Chase
    // ---------------------------
    [Header("Chase")]
    public float chaseRadius = 3.5f;
    public float chaseSpeed = 2.5f;
    public float stopChaseRadius = 4.5f;

    // ---------------------------
    // Damage over time
    // ---------------------------
    [Header("Damage")]
    public int contactDamage = 1;          // dégâts par tick
    public float damageInterval = 1.0f;    // 1 dégât toutes les X secondes

    Rigidbody2D rigidbody2d;
    Animator animator;

    float timer;
    int direction = 1;

    Transform player;
    bool chasing = false;

    // DoT state
    PlayerController playerInContact;
    float damageTimer = 0f;

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

        if (!chasing && dist <= chaseRadius) chasing = true;
        if (chasing && dist >= stopChaseRadius) chasing = false;

        if (!chasing)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                direction = -direction;
                timer = changeTime;
            }
        }

        // Damage over time while touching
        if (playerInContact != null)
        {
            damageTimer -= Time.deltaTime;
            if (damageTimer <= 0f)
            {
                playerInContact.ChangeHealth(-contactDamage);
                damageTimer = damageInterval;
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

            if (animator != null)
            {
                animator.SetFloat("Move X", toPlayer.x);
                animator.SetFloat("Move Y", toPlayer.y);
            }
        }
        else
        {
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

    // Début contact
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc != null)
        {
            playerInContact = pc;
            damageTimer = 0f; // dégâts immédiats au premier contact
        }
    }

    // Contact continu (au cas où Enter ne déclenche pas selon la config)
    void OnTriggerStay2D(Collider2D other)
    {
        if (playerInContact != null) return;

        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc != null)
        {
            playerInContact = pc;
            damageTimer = 0f;
        }
    }

    // Fin contact
    void OnTriggerExit2D(Collider2D other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc != null && pc == playerInContact)
        {
            playerInContact = null;
            damageTimer = 0f;
        }
    }

}
