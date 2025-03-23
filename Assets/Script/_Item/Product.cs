using UnityEngine;

public class Product : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private ItemData itemData;
    public int amount;

    public void SetUp(ItemData itemData, int amount, Inventory inventory)
    {
        this.itemData = itemData;
        this.amount = amount;
        this.inventory = inventory;
    }

    public void AddProduct()
    {
        inventory.AddItem(itemData, amount);
        Destroy(gameObject);
    }
}