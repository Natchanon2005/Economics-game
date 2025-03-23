using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    public TMP_Text itemNameText;
    public TMP_Text priceText;
    public Image iconImage;
    public int defaultAmount;
    private ItemData item;
    private UIManager uIManager;
    private ShopManager shopManager;
    private Inventory inventory;
    private MoneyManager moneyManager;

    // ตั้งค่าข้อมูลไอเทมและเชื่อมกับ ShopManager
    public void Setup(ItemData item, ShopManager shopManager, Inventory inventory, MoneyManager moneyManager, UIManager uIManager)
    {
        this.item = item;
        itemNameText.text = item.itemName;
        priceText.text = item.price.ToString() + " บาท";
        iconImage.sprite = item.icon;
        this.shopManager = shopManager;
        this.inventory = inventory;
        this.moneyManager = moneyManager;
        this.uIManager = uIManager;
    }

    public void BuyItem()
    {
        item.amount = defaultAmount;
        uIManager.ToggleBuyPanel(item);
    }
}
