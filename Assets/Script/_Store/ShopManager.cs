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
    [SerializeField] private TMP_Text economicStateText; // UI text to display the current economic state
    [SerializeField] private Color deflationColor;
    [SerializeField] private Color inflationColor;

    void Start()
    {
        economicStateText.text = $"สถานะ : ปกติ";
        LoadItems(selectedCategory);
        InvokeRepeating(nameof(TriggerRandomEvent), randomEventInterval, randomEventInterval);
    }

    // โหลดข้อมูล ItemData จาก Resources ตามหมวดหมู่
    void LoadItems(ItemCategory category)
    {
        ItemData[] items = Resources.LoadAll<ItemData>("Items");
        allItems = items.Where(item => item.category == category).ToList();

        foreach (Transform child in itemContainer)
        {
            if (child == null) continue;

            var canvasGroup = child.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.DOKill();
                canvasGroup.DOFade(0f, 0.5f).OnComplete(() => Destroy(child?.gameObject));
            }
            else
            {
                Destroy(child?.gameObject);
            }
        }

        int index = 0;
        foreach (var item in allItems)
        {
            var itemObj = Instantiate(itemPrefab, itemContainer);
            var itemUI = itemObj.GetComponent<ShopItemUI>();
            itemUI.Setup(item, this, playerInventory, moneyManager, uIManager);

            int row = index / itemsPerRow;
            int column = index % itemsPerRow;
            Vector2 itemPosition = startPosition + new Vector2(column * itemSpacing.x, -row * itemSpacing.y);

            var rectTransform = itemObj.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = itemPosition;

            var canvasGroup = itemObj.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, 0.5f);

            if (playerInventory != null && itemCountText != null)
            {
                itemCountText.text = $"Owned: {playerInventory.GetItemCount(item)}";
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
            item.price = Mathf.CeilToInt(item.defaultprice * (1 + percentage / 100f)); // Use defaultprice as the base
        }
        LoadItems(selectedCategory); // Refresh UI to reflect new prices
        if (economicStateText != null)
        {
            economicStateText.text = $"สถานะ : เงินเฟ้อ ({percentage}%)";
            economicStateText.color = inflationColor;
        }
    }

    // Adjust item prices for deflation across all categories
    public void ApplyDeflation(float percentage)
    {
        ItemData[] allItemsInResources = Resources.LoadAll<ItemData>("Items");
        foreach (var item in allItemsInResources)
        {
            item.price = Mathf.CeilToInt(item.defaultprice * (1 - percentage / 100f)); // Use defaultprice as the base
            if (item.price < 1) item.price = 1; // Ensure price doesn't drop below 1
        }
        LoadItems(selectedCategory); // Refresh UI to reflect new prices
        if (economicStateText != null)
        {
            economicStateText.text = $"สถานะ : เงินฝืด ({percentage}%)";
            economicStateText.color = deflationColor;
        }
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
