using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData item;
    public int amount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var controller = other.GetComponent<PlayerController>();
        if (controller == null) return;

        // Ajoute à l’inventaire
        if (Inventory.Instance != null && Inventory.Instance.Add(item, amount))
        {
            Destroy(gameObject);
        }
    }
}
