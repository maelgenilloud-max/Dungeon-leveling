using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    public event Action OnChanged;

    public int maxSlots = 24;
    public List<InventorySlotData> items = new();

    public Dictionary<EquipmentSlot, EquipmentItemData> equipped = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (EquipmentSlot s in Enum.GetValues(typeof(EquipmentSlot)))
            equipped[s] = null;
    }

    public bool Add(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;

        if (item.stackable)
        {
            var slot = items.Find(x => x.item == item);
            if (slot != null)
            {
                slot.quantity += amount;
                OnChanged?.Invoke();
                return true;
            }
        }

        if (items.Count >= maxSlots) return false;

        items.Add(new InventorySlotData(item, amount));
        OnChanged?.Invoke();
        return true;
    }

        public int GetCount(ItemData item)
    {
        var slot = items.Find(x => x.item == item);
        return slot != null ? slot.quantity : 0;
    }

    public bool Has(ItemData item, int amount = 1)
    {
        return GetCount(item) >= amount;
    }


    public void Remove(ItemData item, int amount = 1)
    {
        var slot = items.Find(x => x.item == item);
        if (slot == null) return;

        slot.quantity -= amount;
        if (slot.quantity <= 0) items.Remove(slot);

        OnChanged?.Invoke();
    }

    public void Equip(EquipmentItemData eq)
    {
        if (eq == null) return;
        equipped[eq.slot] = eq;
        OnChanged?.Invoke();
    }

    public bool UsePotion(PotionItemData potion, PlayerController player)
{
    if (potion == null || player == null) return false;
    if (player.health >= player.maxHealth) return false;

    player.ChangeHealth(potion.healAmount);
    Remove(potion, 1);
    return true;
}

}
