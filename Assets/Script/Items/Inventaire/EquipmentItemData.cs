using UnityEngine;

[CreateAssetMenu(menuName="Items/Equipment")]
public class EquipmentItemData : ItemData
{
    public EquipmentSlot slot;

    private void OnEnable()
    {
        type = ItemType.Equipment;
        stackable = false;
    }
}
