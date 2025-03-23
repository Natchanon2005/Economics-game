using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using TMPro; // Ensure this is included for text handling

public class ShopManager : MonoBehaviour
{
    public List<ItemData> allItems; // รายการไอเทมทั้งหมด
    public Transform itemContainer; // ตำแหน่งวาง UI สินค้า
    public GameObject itemPrefab; // Prefab ของ UI ที่ใช้แสดงสินค้า
    public ItemCategory selectedCategory; // หมวดหมู่ที่เลือก
    public Vector2 itemSpacing = new Vector2(100, 100); // ระยะห่างระหว่างไอเทม
    public int itemsPerRow = 3; // จำนวนไอเทมต่อแถว
    public Vector2 startPosition = new Vector2(0, 0); // ตำแหน่งเริ่มต้นของไอเทม
    [SerializeField] private UIManager uIManager;
    [SerializeField] private Inventory playerInventory; // Reference to the player's inventory
    [SerializeField] private TMP_Text itemCountText; // UI text to display item count
    [SerializeField] private MoneyManager moneyManager;
    [SerializeField] private float randomEventInterval = 60f; // Interval in seconds for random events
    [SerializeField] private float inflationPercentage = 10f; // Percentage for inflation
    [SerializeField] private float deflationPercentage = 10f; // Percentage for deflation

    void Start()
    {
        LoadItems(selectedCategory);
        InvokeRepeating(nameof(TriggerRandomEvent), randomEventInterval, randomEventInterval);
    }

    // โหลดข้อมูล ItemData จาก Resources ตามหมวดหมู่
    void LoadItems(ItemCategory category)
    {
        // โหลดทุกไฟล์ ItemData จากโฟลเดอร์ Resources/Items
        ItemData[] items = Resources.LoadAll<ItemData>("Items");

        // กรองข้อมูลตามหมวดหมู่
        allItems = items.Where(item => item.category == category).ToList();

        // ลบ UI เดิมทั้งหมด พร้อม fade out
        foreach (Transform child in itemContainer)
        {
            if (child == null) continue; // Ensure the child is not null

            var canvasGroup = child.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                // Kill any existing DOTween animations on the canvasGroup
                canvasGroup.DOKill();

                canvasGroup.DOFade(0f, 0.5f).OnComplete(() =>
                {
                    if (child != null) // Check again if the object still exists
                    {
                        Destroy(child.gameObject);
                    }
                });
            }
            else
            {
                if (child != null) // Check if the object still exists
                {
                    Destroy(child.gameObject);
                }
            }
        }

        // สร้าง UI สำหรับแต่ละไอเทม
        int index = 0; // ตัวนับตำแหน่งไอเทม
        foreach (var item in allItems)
        {
            // สร้าง UI สำหรับแต่ละไอเทม
            var itemObj = Instantiate(itemPrefab, itemContainer);
            var itemUI = itemObj.GetComponent<ShopItemUI>();
            itemUI.Setup(item, this, playerInventory, moneyManager, uIManager);

            // คำนวณตำแหน่งของไอเทมในกริด
            int row = index / itemsPerRow;
            int column = index % itemsPerRow;
            Vector2 itemPosition = startPosition + new Vector2(column * itemSpacing.x, -row * itemSpacing.y);

            // ตั้งค่าตำแหน่งของ UI ไอเทม
            itemObj.GetComponent<RectTransform>().anchoredPosition = itemPosition;

            // เพิ่ม fade in effect
            var canvasGroup = itemObj.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, 0.5f);

            // Update item count in the shop UI
            if (playerInventory != null && itemCountText != null)
            {
                int itemCount = playerInventory.GetItemCount(item);
                itemCountText.text = $"Owned: {itemCount}";
            }

            index++;
        }
    }

    // เปลี่ยนหมวดหมู่และโหลดไอเทมใหม่
    public void ChangeCategory(ItemCategory newCategory)
    {
        if (selectedCategory == newCategory) return;
        selectedCategory = newCategory;
        LoadItems(selectedCategory);
    }

    // Adjust item prices for inflation across all categories
    public void ApplyInflation(float percentage)
    {
        ItemData[] allItemsInResources = Resources.LoadAll<ItemData>("Items");
        foreach (var item in allItemsInResources)
        {
            item.price = Mathf.CeilToInt(item.price * (1 + percentage / 100f));
        }
        LoadItems(selectedCategory); // Refresh UI to reflect new prices
    }

    // Adjust item prices for deflation across all categories
    public void ApplyDeflation(float percentage)
    {
        ItemData[] allItemsInResources = Resources.LoadAll<ItemData>("Items");
        foreach (var item in allItemsInResources)
        {
            item.price = Mathf.CeilToInt(item.price * (1 - percentage / 100f));
            if (item.price < 1) item.price = 1; // Ensure price doesn't drop below 1
        }
        LoadItems(selectedCategory); // Refresh UI to reflect new prices
    }

    public float GetInflationMultiplier()
    {
        return 1 + inflationPercentage / 100f;
    }

    private void TriggerRandomEvent()
    {
        float randomPercentage = Random.Range(10f, 100f); // Generate a random percentage between 10 and 100

        if (Random.value > 0.5f)
        {
            ApplyInflation(randomPercentage);
            Debug.Log($"Inflation applied: Prices increased by {randomPercentage}%");
        }
        else
        {
            ApplyDeflation(randomPercentage);
            Debug.Log($"Deflation applied: Prices decreased by {randomPercentage}%");
        }
    }
}
