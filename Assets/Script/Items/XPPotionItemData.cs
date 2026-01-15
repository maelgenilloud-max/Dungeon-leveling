using UnityEngine;

[CreateAssetMenu(menuName="Items/XP Potion")]
public class XpPotionItemData : ItemData
{
    public int xpPointsGained = 1;

    private void OnEnable()
    {
        stackable = true;
    }
}
