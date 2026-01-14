using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform gridParent;
    [SerializeField] private InventorySlotUI slotPrefab;
    [SerializeField] private TMP_Text selectedName;

    private ItemData selectedItem;

    void OnEnable()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.OnChanged += Rebuild;

        Rebuild();
    }

    void OnDisable()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.OnChanged -= Rebuild;
    }

    public void Rebuild()
    {
        for (int i = gridParent.childCount - 1; i >= 0; i--)
            Destroy(gridParent.GetChild(i).gameObject);

        if (Inventory.Instance == null) return;

        foreach (var slot in Inventory.Instance.items)
        {
            var ui = Instantiate(slotPrefab, gridParent);
            ui.Init(this);
            ui.Bind(slot.item, slot.quantity);
        }

        if (selectedName != null)
            selectedName.text = selectedItem != null ? selectedItem.itemName : "No selection";
    }

    public void SelectItem(ItemData item)
    {
        selectedItem = item;

        if (selectedName != null)
            selectedName.text = selectedItem != null ? selectedItem.itemName : "No selection";
    }

    public void OnUseButtonClicked()
    {
        if (selectedItem is not PotionItemData potion) return;

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null) return;

        Inventory.Instance.UsePotion(potion, player);
    }
}
