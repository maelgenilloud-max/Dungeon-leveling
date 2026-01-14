using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text qtyText;

    private ItemData item;
    private InventoryUI ui;

    public void Init(InventoryUI uiRef)
    {
        ui = uiRef;
    }

    public void Bind(ItemData itemData, int qty)
    {
        item = itemData;

        icon.sprite = itemData.icon;
        icon.enabled = itemData.icon != null;

        qtyText.text = itemData.stackable ? qty.ToString() : "";
    }

    public void OnClick()
    {
        ui.SelectItem(item);
    }
}
