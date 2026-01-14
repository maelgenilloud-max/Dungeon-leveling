using UnityEngine;

[CreateAssetMenu(menuName="Items/Potion")]
public class PotionItemData : ItemData
{
    public int healAmount = 1;

    private void OnEnable()
    {
        type = ItemType.Potion;
        stackable = true;
    }
}
