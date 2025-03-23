using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    public TMP_Text itemNameText;
    public TMP_Text priceText;
    public Image iconImage;
    private ItemData item;
    private ShopManager shopManager;
    private Inventory inventory;
    private MoneyManager moneyManager;

    // ตั้งค่าข้อมูลไอเทมและเชื่อมกับ ShopManager
    public void Setup(ItemData item, ShopManager shopManager, Inventory inventory, MoneyManager moneyManager)
    {
        this.item = item;
        itemNameText.text = item.itemName;
        priceText.text = item.price.ToString() + " Gold";
        iconImage.sprite = item.icon;
        this.shopManager = shopManager;
        this.inventory = inventory;
        this.moneyManager = moneyManager;

    }

    public void BuyItem(int amount)
    {
        if (moneyManager.TrySpendMoney(item.price))
        {
            inventory.AddItem(item, amount);
        }
    }
}
