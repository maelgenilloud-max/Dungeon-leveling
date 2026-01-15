using UnityEngine;

public class EnemyDamageTrigger : MonoBehaviour
{
    public int damage = 1;
    public float damageInterval = 0.5f;

    float timer;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Debug utile pour vérifier que le trigger touche bien quelque chose
        Debug.Log("[EnemyDamageTrigger] Enter: " + other.name);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        // IMPORTANT: marche même si le collider est sur un child
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player != null)
        {
            Debug.Log("[EnemyDamageTrigger] Damage to player");
            player.ChangeHealth(-damage);
            timer = damageInterval;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>() != null)
            timer = 0f;
    }
}
