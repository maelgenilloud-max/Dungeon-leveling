using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform gridParent;
    [SerializeField] private InventorySlotUI slotPrefab;
    [SerializeField] private TMP_Text selectedName;

    [Header("Grid")]
    [SerializeField] private int slotCount = 27; // 9 x 3

    private readonly List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();
    private ItemData selectedItem;

    void OnEnable()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.OnChanged += Rebuild;

        EnsureSlotsExist();
        Rebuild();
    }

    void OnDisable()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.OnChanged -= Rebuild;
    }

    void EnsureSlotsExist()
    {
        if (slotUIs.Count > 0) return;

        for (int i = 0; i < slotCount; i++)
        {
            var ui = Instantiate(slotPrefab, gridParent);
            ui.Init(this);
            ui.Clear();
            slotUIs.Add(ui);
        }
    }

    public void Rebuild()
    {
        EnsureSlotsExist();
        Debug.Log("InventoryUI.Rebuild called");
        Debug.Log("Inventory items count = " + Inventory.Instance.items.Count);


        // 1) vider tous les slots
        for (int i = 0; i < slotUIs.Count; i++)
            slotUIs[i].Clear();

        // 2) remplir avec les items (dans l’ordre, à partir de 0)
        if (Inventory.Instance != null)
        {
            int i = 0;
            foreach (var slot in Inventory.Instance.items)
            {
                if (i >= slotUIs.Count) break;
                slotUIs[i].Bind(slot.item, slot.quantity);
                i++;
            }
        }

        // 3) texte sélection
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
        if (selectedItem == null) return;

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null) return;

        if (selectedItem is PotionItemData potion)
        {
            Inventory.Instance.UsePotion(potion, player);
            return;
        }

        if (selectedItem is XpPotionItemData xpPotion)
        {
            Inventory.Instance.UseXpPotion(xpPotion, player);
            return;
        }
    }
}
