using UnityEngine;

public enum ItemCategory
{
    Coffee_beans,
    Milk,
    Topping,
    Flavoring
}

[CreateAssetMenu(fileName = "New Item", menuName = "Shop/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public int price;
    public ItemCategory category;
    public Sprite icon;
    public int amount;
    public int defaultAmount;
}
