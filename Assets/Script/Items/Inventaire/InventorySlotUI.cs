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

        PlayerController player = Object.FindFirstObjectByType<PlayerController>();
        if (player == null) return;

        // Potion de vie
        if (item is PotionItemData potion)
        {
            Inventory.Instance.UsePotion(potion, player);
            return;
        }

        // Potion d'XP
        if (item is XpPotionItemData xpPotion)
        {
            Inventory.Instance.UseXpPotion(xpPotion, player);
            return;
        }

        // Sinon, sélectionner (équipement, etc.)
        inventoryUI.SelectItem(item);
    }
}
