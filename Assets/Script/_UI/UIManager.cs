using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;
using TMPro;

[Serializable]
public class FadeSetting
{
    public CanvasGroup canvasGroup;
    public float duration;
    public float delay;
}

public class UIManager : MonoBehaviour
{
    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private CameraHeadBobbing cameraHeadBobbing;
    [Space]
    [Header("Shop Panel")]
    [SerializeField] private bool shopOpen = false;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Vector2 shopAnimationAfter;
    [SerializeField] private Vector2 shopAnimationBefore;
    [SerializeField] private float shopDuration = 1f;
    private PlayerInputActions _inputActions;
    private PlayerInputActions.UIActions _input;
    private bool isAnimating = false;
    private float animationCooldown = 0.5f; // Cooldown duration
    private float lastAnimationTime = 0f;

    [Header("Fade Settings")]
    [SerializeField] private List<FadeSetting> fadeSettings;

    [Header("Order Panel")]
    [SerializeField] private GameObject orderPanel;
    [SerializeField] private Vector2 orderAnimationAfter;
    [SerializeField] private Vector2 orderAnimationBefore;
    [SerializeField] private float orderDuration = 1f;
    [SerializeField] private bool orderOpen = false;

    [Header("Buy Settings")]
    [SerializeField] private GameObject buyPanel;
    [SerializeField] private TMP_Text buyName;
    [SerializeField] private TMP_Text buyAmount;
    [SerializeField] private TMP_Text buyPrice;
    [SerializeField] private Vector2 buyAnimationAfter;
    [SerializeField] private Vector2 buyAnimationBefore;
    [SerializeField] private float buyDuration = 1f;
    [SerializeField] private bool buyOpen = false;
    [SerializeField] private ItemData itemData;

    [Header("Bank Panel")]
    [SerializeField] private GameObject bankPanel;
    [SerializeField] private Vector2 bankAnimationAfter;
    [SerializeField] private Vector2 bankAnimationBefore;
    [SerializeField] private float bankDuration = 1f;
    [SerializeField] private bool bankOpen = false;

    [Header("Coffee Panel")]
    [SerializeField] private GameObject coffeePanel;
    [SerializeField] private Vector2 coffeeAnimationAfter;
    [SerializeField] private Vector2 coffeeAnimationBefore;
    [SerializeField] private float coffeeDuration = 1f;
    [SerializeField] private bool coffeeOpen = false;

    [Header("How To Panel")]
    [SerializeField] private GameObject howToPanel;
    [SerializeField] private Vector2 howToAnimationAfter;
    [SerializeField] private Vector2 howToAnimationBefore;
    [SerializeField] private float howToDuration = 1f;
    [SerializeField] private bool howToOpen = false;

    [SerializeField] public Inventory Inventory; // Expose Inventory
    [SerializeField] public ShopManager ShopManager; // Expose ShopManager

    void Start()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
        _input = _inputActions.UI;

        if (shopPanel == null)
        {
            Debug.LogWarning("ShopPanel is not assigned in the inspector.");
            return;
        }

        shopPanel.SetActive(shopOpen);
        orderPanel.SetActive(orderOpen);
        buyPanel.SetActive(buyOpen);
        bankPanel.SetActive(bankOpen);
        coffeePanel.SetActive(coffeeOpen);
        howToPanel.SetActive(howToOpen);

        // Bind the ToggleShop function to the OpenStore action
        _input.OpenStore.performed += ctx => ToggleShop();
        _input.OpenBank.performed += ctx => ToggleBankPanel();
        _input.OpenHowTo.performed += ctx => ToggleHowToPanel();
    }

    public void AnimateUI(GameObject panel, bool open, Vector2 targetPosition, float duration)
    {
        if (panel == null)
        {
            Debug.LogWarning("Panel is null. Cannot animate UI.");
            return;
        }

        isAnimating = true;

        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogWarning("RectTransform component is missing on the panel.");
            isAnimating = false;
            return;
        }

        if (open) panel.SetActive(true);

        rectTransform.DOAnchorPos(targetPosition, duration).OnComplete(() =>
        {
            if (!open) panel.SetActive(false);
            isAnimating = false;
        });
    }

    private void ToggleShop()
    {
        if (Time.time - lastAnimationTime < animationCooldown || isAnimating || orderOpen || buyOpen) return;
        lastAnimationTime = Time.time;

        shopOpen = !shopOpen;
        bool isShopClosed = !shopOpen;

        cameraHeadBobbing.enabled = isShopClosed;
        playerCamera.SetCameraControl(isShopClosed);
        playerCamera.LockState = shopOpen ? LockState.None : LockState.Lock;
        playerCamera.VisibleState = shopOpen ? VisibleState.Hidden : VisibleState.Show;
        playerCharacter.SetMovementEnabled(isShopClosed);
        playerCharacter.SetJumpEnabled(isShopClosed);
        playerCharacter.SetCrouchEnabled(isShopClosed);
        AnimateUI(shopPanel, shopOpen, shopOpen ? shopAnimationAfter : shopAnimationBefore, shopDuration);
        playerCamera.SetDepthOfField(shopOpen ? DepthOfFieldState.Enabled : DepthOfFieldState.Disabled);
    }

    public void ToggleOrderPanel()
    {
        if (Time.time - lastAnimationTime < animationCooldown || isAnimating || shopOpen || buyOpen) return;
        lastAnimationTime = Time.time;

        orderOpen = !orderOpen;
        bool isOrderClosed = !orderOpen;

        cameraHeadBobbing.enabled = isOrderClosed;
        playerCamera.SetCameraControl(isOrderClosed);
        playerCamera.LockState = orderOpen ? LockState.None : LockState.Lock;
        playerCamera.VisibleState = orderOpen ? VisibleState.Hidden : VisibleState.Show;
        playerCharacter.SetMovementEnabled(isOrderClosed);
        playerCharacter.SetJumpEnabled(isOrderClosed);
        playerCharacter.SetCrouchEnabled(isOrderClosed);
        AnimateUI(orderPanel, orderOpen, orderOpen ? orderAnimationAfter : orderAnimationBefore, orderDuration);
        playerCamera.SetDepthOfField(orderOpen ? DepthOfFieldState.Enabled : DepthOfFieldState.Disabled);
    }

    public void ToggleBuyPanel(ItemData itemData)
    {
        if (Time.time - lastAnimationTime < animationCooldown || isAnimating || orderOpen) return;
        lastAnimationTime = Time.time;

        this.itemData = itemData;
        buyName.text = itemData.itemName;
        UpdateAmount(itemData);
        buyOpen = !buyOpen;

        AnimateUI(buyPanel, buyOpen, buyOpen ? buyAnimationAfter : buyAnimationBefore, buyDuration);
    }

    public void ToggleBuyPanelForButton()
    {
        ToggleBuyPanel(itemData);
    }

    public void UpdateAmount(ItemData itemData)
    {
        int amountforprice = itemData.amount / itemData.defaultAmount;
        buyPrice.text = $"ราคา {itemData.price * amountforprice}"; // Fix calculation
        buyAmount.text = $"{itemData.amount}";
    }

    public void AddAmount()
    {
        itemData.amount += itemData.defaultAmount;
        UpdateAmount(itemData);
    }

    public void reduceAmount()
    {
        if (itemData.amount <= 0) return; // Prevent negative values
        itemData.amount -= itemData.defaultAmount;
        if (itemData.amount < 0) itemData.amount = 0; // Ensure amount is not negative
        UpdateAmount(itemData);
    }

    public void BuyItem()
    {
        Inventory.Buy(itemData);
    }

    private void OnDestroy()
    {
        _inputActions.Disable();
    }

    public void FadeIn(List<FadeSetting> fadeSettings)
    {
        if (fadeSettings == null || fadeSettings.Count == 0) return;

        foreach (var setting in fadeSettings)
        {
            if (setting.canvasGroup == null) continue;

            setting.canvasGroup.DOFade(1f, setting.duration).SetDelay(setting.delay).OnStart(() =>
            {
                setting.canvasGroup.interactable = true;
                setting.canvasGroup.blocksRaycasts = true;
            });
        }
    }

    public void FadeOut(List<FadeSetting> fadeSettings)
    {
        if (fadeSettings == null || fadeSettings.Count == 0) return;

        foreach (var setting in fadeSettings)
        {
            if (setting.canvasGroup == null) continue;

            setting.canvasGroup.DOFade(0f, setting.duration).SetDelay(setting.delay).OnComplete(() =>
            {
                setting.canvasGroup.interactable = false;
                setting.canvasGroup.blocksRaycasts = false;
            });
        }
    }

    public void ToggleBankPanel()
    {
        if (Time.time - lastAnimationTime < animationCooldown || isAnimating || shopOpen || orderOpen || buyOpen) return;
        lastAnimationTime = Time.time;

        bankOpen = !bankOpen;
        bool isBankClosed = !bankOpen;

        cameraHeadBobbing.enabled = isBankClosed;
        playerCamera.SetCameraControl(isBankClosed);
        playerCamera.LockState = bankOpen ? LockState.None : LockState.Lock;
        playerCamera.VisibleState = bankOpen ? VisibleState.Hidden : VisibleState.Show;
        playerCharacter.SetMovementEnabled(isBankClosed);
        playerCharacter.SetJumpEnabled(isBankClosed);
        playerCharacter.SetCrouchEnabled(isBankClosed);
        AnimateUI(bankPanel, bankOpen, bankOpen ? bankAnimationAfter : bankAnimationBefore, bankDuration);
        playerCamera.SetDepthOfField(bankOpen ? DepthOfFieldState.Enabled : DepthOfFieldState.Disabled);
    }

    public void ToggleCoffeePanel()
    {
        if (Time.time - lastAnimationTime < animationCooldown || isAnimating || shopOpen || orderOpen || buyOpen || bankOpen) return;
        lastAnimationTime = Time.time;

        coffeeOpen = !coffeeOpen;
        bool isCoffeeClosed = !coffeeOpen;

        cameraHeadBobbing.enabled = isCoffeeClosed;
        playerCamera.SetCameraControl(isCoffeeClosed);
        playerCamera.LockState = coffeeOpen ? LockState.None : LockState.Lock;
        playerCamera.VisibleState = coffeeOpen ? VisibleState.Hidden : VisibleState.Show;
        playerCharacter.SetMovementEnabled(isCoffeeClosed);
        playerCharacter.SetJumpEnabled(isCoffeeClosed);
        playerCharacter.SetCrouchEnabled(isCoffeeClosed);
        AnimateUI(coffeePanel, coffeeOpen, coffeeOpen ? coffeeAnimationAfter : coffeeAnimationBefore, coffeeDuration);
        playerCamera.SetDepthOfField(coffeeOpen ? DepthOfFieldState.Enabled : DepthOfFieldState.Disabled);
    }

    public void ToggleHowToPanel()
    {
        if (Time.time - lastAnimationTime < animationCooldown || isAnimating || shopOpen || orderOpen || buyOpen || bankOpen || coffeeOpen) return;
        lastAnimationTime = Time.time;

        howToOpen = !howToOpen;
        bool isHowToClosed = !howToOpen;

        cameraHeadBobbing.enabled = isHowToClosed;
        playerCamera.SetCameraControl(isHowToClosed);
        playerCamera.LockState = howToOpen ? LockState.None : LockState.Lock;
        playerCamera.VisibleState = howToOpen ? VisibleState.Hidden : VisibleState.Show;
        playerCharacter.SetMovementEnabled(isHowToClosed);
        playerCharacter.SetJumpEnabled(isHowToClosed);
        playerCharacter.SetCrouchEnabled(isHowToClosed);
        AnimateUI(howToPanel, howToOpen, howToOpen ? howToAnimationAfter : howToAnimationBefore, howToDuration);
        playerCamera.SetDepthOfField(howToOpen ? DepthOfFieldState.Enabled : DepthOfFieldState.Disabled);
    }
}
