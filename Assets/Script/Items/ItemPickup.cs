using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData item;
    public int amount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[PICKUP] Trigger enter by: {other.name}");

        if (other.GetComponent<PlayerController>() == null)
        {
            Debug.Log("[PICKUP] Not the player");
            return;
        }

        Debug.Log($"[PICKUP] Item={(item ? item.itemName : "NULL")} amount={amount}");
        Debug.Log($"[PICKUP] Inventory.Instance={(Inventory.Instance != null)}");

        if (Inventory.Instance != null && item != null)
        {
            Inventory.Instance.Add(item, amount);
            Debug.Log("[PICKUP] Added -> destroying pickup");
            Destroy(gameObject);
        }
    }
}
