using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text qtyText;

    private ItemData item;
    private InventoryUI inventoryUI;

    public void Init(InventoryUI ui)
    {
        inventoryUI = ui;
    }

    public void Bind(ItemData newItem, int quantity)
    {
        item = newItem;

        Debug.Log($"BIND slot: item={(item ? item.itemName : "NULL")} icon={(item != null && item.icon != null ? item.icon.name : "NULL")} qty={quantity}");

        icon.sprite = (item != null) ? item.icon : null;
        icon.enabled = (icon.sprite != null);

        // Pour le test, affiche aussi 1
        qtyText.text = quantity.ToString();
    }

    public void Clear()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        qtyText.text = "";
    }

    public void OnClick()
    {
        if (item == null) return;

        // Si c'est une potion, on l'utilise directement
        if (item is PotionItemData potion)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player == null) return;

            Inventory.Instance.UsePotion(potion, player);
            return;
        }

        // Sinon, juste sélectionner (pour équipements, etc.)
        inventoryUI.SelectItem(item);
    }

}
