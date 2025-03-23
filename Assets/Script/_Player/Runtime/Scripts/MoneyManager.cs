using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    private float currentMoney;
    private TMP_Text moneyText;

    public float CurrentMoney => currentMoney;

    public void Initialize(float initialMoney, TMP_Text moneyTextComponent)
    {
        currentMoney = initialMoney;
        moneyText = moneyTextComponent;
        UpdateMoneyDisplay();
    }

    public bool TrySpendMoney(float amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateMoneyDisplay();
            return true;
        }
        return false;
    }

    public void AddMoney(float amount)
    {
        currentMoney += amount;
        UpdateMoneyDisplay();
    }

    public void UpdateMoneyDisplay()
    {
        if (moneyText == null) return;
        moneyText.text = $"{Mathf.RoundToInt(currentMoney)} บาท";
    }
}
