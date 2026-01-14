using UnityEngine;

[CreateAssetMenu(menuName="Items/Kunai")]
public class KunaiItemData : ItemData
{
    private void OnEnable()
    {
        type = ItemType.Equipment;   // ou Potion, peu importe ici
        stackable = true;
    }
}
