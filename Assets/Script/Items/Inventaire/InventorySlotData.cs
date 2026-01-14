[System.Serializable]
public class InventorySlotData
{
    public ItemData item;
    public int quantity;

    public InventorySlotData(ItemData item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}
