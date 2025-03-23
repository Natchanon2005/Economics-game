using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private Dictionary<ItemCategory, Dictionary<string, int>> categoryItemCounts = new Dictionary<ItemCategory, Dictionary<string, int>>();
    [SerializeField] private TMP_Text coffeeBeansText;
    [SerializeField] private TMP_Text milkText;
    [SerializeField] private TMP_Text toppingText;
    [SerializeField] private TMP_Text flavoringText;
    private Dictionary<string, int> drinkCounts = new Dictionary<string, int>();
    [SerializeField] private MoneyManager moneyManager; // Reference to MoneyManager
    [SerializeField] private UIManager uIManager;
    private Dictionary<string, float> baseDrinkPrices = new Dictionary<string, float>
    {
        { "Black Dragon Brew", 45f },
        { "Moonlight Caramel Latte", 60f },
        { "Frosted Vanilla Heaven", 70f },
        { "Liberica Lava Burst", 65f }
    };

    [System.Serializable]
    private class DrinkCount
    {
        public string drinkName;
        public int count;
    }

    [SerializeField] private List<DrinkCount> drinkCountList = new List<DrinkCount>();

    private void Start()
    {
        // Initialize counts for all categories
        foreach (ItemCategory category in System.Enum.GetValues(typeof(ItemCategory)))
        {
            categoryItemCounts[category] = new Dictionary<string, int>();
        }
        UpdateAllCategoryTexts();
    }

    public void AddItem(ItemData item, int count)
    {
        if (!categoryItemCounts[item.category].ContainsKey(item.itemName))
        {
            categoryItemCounts[item.category][item.itemName] = 0;
        }
        categoryItemCounts[item.category][item.itemName] += count;

        Debug.Log($"Added {count} of {item.itemName} to category {item.category}. Current count: {categoryItemCounts[item.category][item.itemName]}");

        if (item.itemName == "นมข้นหวาน")
        {
            Debug.Log($"Special log: นมข้นหวาน count is now {categoryItemCounts[item.category][item.itemName]}.");
        }

        UpdateCategoryText(item.category);
    }

    public int GetItemCount(ItemData item)
    {
        if (categoryItemCounts.ContainsKey(item.category) && categoryItemCounts[item.category].ContainsKey(item.itemName))
        {
            return categoryItemCounts[item.category][item.itemName];
        }
        return 0;
    }

    public void Buy(ItemData itemData)
    {
        if (moneyManager.TrySpendMoney(itemData.price))
        {
            AddItem(itemData, itemData.amount);
            uIManager.ToggleBuyPanel(itemData);
        }
    }

    private void UpdateCategoryText(ItemCategory category)
    {
        string text = "";
        foreach (var item in categoryItemCounts[category])
        {
            text += $"{item.Key}: {item.Value}\n";
        }

        switch (category)
        {
            case ItemCategory.Coffee_beans:
                if (coffeeBeansText != null)
                    coffeeBeansText.text = text;
                break;
            case ItemCategory.Milk:
                if (milkText != null)
                    milkText.text = text;
                break;
            case ItemCategory.Topping:
                if (toppingText != null)
                    toppingText.text = text;
                break;
            case ItemCategory.Flavoring:
                if (flavoringText != null)
                    flavoringText.text = text;
                break;
        }
    }

    private void UpdateAllCategoryTexts()
    {
        foreach (ItemCategory category in categoryItemCounts.Keys)
        {
            UpdateCategoryText(category);
        }
    }

    // Example method to add 15 coffee beans of a specific type
    public void AddFifteenCoffeeBeans(string coffeeBeanItem)
    {
        CreateDrink(coffeeBeanItem);
    }

    public bool CreateDrink(string drinkName)
    {
        Dictionary<string, int> requiredIngredients = GetDrinkRecipe(drinkName);

        if (requiredIngredients == null)
        {
            Debug.LogWarning($"Drink recipe for {drinkName} not found.");
            return false;
        }

        // Check if all ingredients are available
        foreach (var ingredient in requiredIngredients)
        {
            ItemData item = ScriptableObject.CreateInstance<ItemData>();
            item.itemName = ingredient.Key;
            item.category = GetCategoryForItem(ingredient.Key);
            if (GetItemCount(item) < ingredient.Value)
            {
                Debug.LogWarning($"Not enough {ingredient.Key} to create {drinkName}. Current count: {GetItemCount(item)}, Required: {ingredient.Value}");
                return false;
            }
        }

        // Deduct ingredients
        foreach (var ingredient in requiredIngredients)
        {
            ItemData item = ScriptableObject.CreateInstance<ItemData>();
            item.itemName = ingredient.Key;
            item.category = GetCategoryForItem(ingredient.Key);
            AddItem(item, -ingredient.Value);
        }

        // Increment drink count
        if (!drinkCounts.ContainsKey(drinkName))
        {
            drinkCounts[drinkName] = 0;
        }
        drinkCounts[drinkName]++;
        Debug.Log($"{drinkName} created successfully.");

        // Update the serialized list for the editor
        UpdateDrinkCountList();

        return true;
    }

    private void UpdateDrinkCountList()
    {
        drinkCountList.Clear();
        foreach (var drink in drinkCounts)
        {
            drinkCountList.Add(new DrinkCount { drinkName = drink.Key, count = drink.Value });
        }
    }

    private Dictionary<string, int> GetDrinkRecipe(string drinkName)
    {
        switch (drinkName)
        {
            case "Black Dragon Brew":
                return new Dictionary<string, int>
                {
                    { "เมล็ดกาแฟโรบัสต้า", 20 },
                    { "น้ำตาลทราย", 5 },
                    { "ครีมเทียม", 10 }
                };
            case "Moonlight Caramel Latte":
                return new Dictionary<string, int>
                {
                    { "เมล็ดกาแฟอาราบิก้า", 18 },
                    { "นมสด", 150 },
                    { "คาราเมลป๊อปคอร์น", 5 },
                    { "ไซรัป", 5 },
                    { "วิปครีม", 10 }
                };
            case "Frosted Vanilla Heaven":
                return new Dictionary<string, int>
                {
                    { "เมล็ดกาแฟเอ็กเซลซ่า", 15 },
                    { "นมโอ๊ต", 150 },
                    { "ไอศกรีมวานิลลา", 20 },
                    { "มาร์ชเมลโล่", 3 },
                    { "น้ำตาลทราย", 7 },
                    { "วิปครีม", 10 }
                };
            case "Liberica Lava Burst":
                return new Dictionary<string, int>
                {
                    { "เมล็ดกาแฟลิเบอริก้า", 22 },
                    { "นมจืด", 200 },
                    { "คาราเมลป๊อปคอร์น", 3 },
                    { "ไอศกรีมวานิลลา", 15 },
                    { "ไซรัป", 5 }
                };
            default:
                return null;
        }
    }

    private ItemCategory GetCategoryForItem(string itemName)
    {
        // Map item names to their categories
        switch (itemName)
        {
            case "เมล็ดกาแฟโรบัสต้า":
            case "เมล็ดกาแฟอาราบิก้า":
            case "เมล็ดกาแฟเอ็กเซลซ่า":
            case "เมล็ดกาแฟลิเบอริก้า":
                return ItemCategory.Coffee_beans;
            case "นมสด":
            case "นมโอ๊ต":
            case "นมจืด":
                return ItemCategory.Milk;
            case "ครีมเทียม":
            case "นมข้นหวาน":
            case "น้ำตาลทราย":
            case "ไซรัป":
                return ItemCategory.Flavoring;
            case "คาราเมลป๊อปคอร์น":
            case "ไอศกรีมวานิลลา":
            case "มาร์ชเมลโล่":
            case "วิปครีม":
                return ItemCategory.Topping;
            default:
                Debug.LogError($"Category for item {itemName} not found.");
                return default;
        }
    }

    public int GetDrinkCount(string drinkName)
    {
        return drinkCounts.ContainsKey(drinkName) ? drinkCounts[drinkName] : 0;
    }

    public bool SellDrink(string drinkName, float inflationMultiplier, int quantity)
    {
        if (!drinkCounts.ContainsKey(drinkName) || drinkCounts[drinkName] < quantity)
        {
            Debug.LogWarning($"Not enough {drinkName} available to sell.");
            return false;
        }

        if (!baseDrinkPrices.ContainsKey(drinkName))
        {
            Debug.LogWarning($"Price for {drinkName} not found.");
            return false;
        }

        // Calculate the total adjusted price
        float adjustedPrice = baseDrinkPrices[drinkName] * inflationMultiplier * quantity;

        // Add money to the player's balance
        moneyManager.AddMoney(adjustedPrice);

        // Decrease the drink count
        drinkCounts[drinkName] -= quantity;
        Debug.Log($"{quantity} {drinkName}(s) sold for {adjustedPrice}.");

        // Update the serialized list for the editor
        UpdateDrinkCountList();

        return true;
    }
}
