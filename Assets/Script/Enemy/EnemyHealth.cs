using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHP = 5;
    private int hp;

    [Header("Drops (optional)")]
    public EnemyDropper dropper;

    private void Awake()
    {
        hp = maxHP;
        if (dropper == null) dropper = GetComponent<EnemyDropper>();
    }

    public void TakeDamage(int amount)
    {
        amount = Mathf.Max(1, amount);
        hp -= amount;

        Debug.Log($"[EnemyHealth] Took {amount} damage. HP={hp}/{maxHP}");

        if (hp <= 0) Die();
    }

    private void Die()
    {
        if (dropper != null)
            dropper.Drop();

        Destroy(gameObject);
    }
}
