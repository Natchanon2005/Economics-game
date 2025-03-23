using UnityEngine;
using TMPro; // Required for TMP_Text
using UnityEngine.SceneManagement; // Required for scene management

public class Bank : MonoBehaviour
{
    [SerializeField] private TMP_Text remainingAmountText; // TMP_Text to display remaining amount
    [SerializeField] private TMP_InputField paymentInputField; // TMP_InputField for user input
    [SerializeField] private string nextSceneName; // Name of the scene to load
    [SerializeField] private MoneyManager moneyManager;
    private int totalPaid = 0; // Tracks the total amount paid
    private const int targetAmount = 10000; // Target amount to trigger scene change

    void Start()
    {
        UpdateRemainingAmount();
    }

    public void OnPayButtonClicked()
    {
        if (int.TryParse(paymentInputField.text, out int amount))
        {
            if (moneyManager.TrySpendMoney(amount))
            {
                Pay(amount);
                paymentInputField.text = ""; // Clear the input field after processing
            }
        }
        else
        {
            Debug.LogWarning("Invalid input. Please enter a valid number.");
        }
    }

    public void Pay(int amount)
    {
        totalPaid += amount;
        UpdateRemainingAmount();

        if (totalPaid >= targetAmount)
        {
            LoadNextScene();
        }
    }

    private void UpdateRemainingAmount()
    {
        int remaining = targetAmount - totalPaid;
        remainingAmountText.text = $"ปลดหนี้อีก : {remaining} บาท";
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}