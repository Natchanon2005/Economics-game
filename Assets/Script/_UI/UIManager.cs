using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;

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

    [Header("Fade Settings")]
    [SerializeField] private List<FadeSetting> fadeSettings;

    [Header("Order Panel")]
    [SerializeField] private GameObject orderPanel;
    [SerializeField] private Vector2 orderAnimationAfter;
    [SerializeField] private Vector2 orderAnimationBefore;
    [SerializeField] private float orderDuration = 1f;
    [SerializeField] private bool orderOpen = false;

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

        // Bind the ToggleShop function to the OpenStore action
        _input.OpenStore.performed += ctx => ToggleShop();
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
        if (isAnimating) return;
        if (orderOpen) return;

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
        if (isAnimating) return;
        if (shopOpen) return;
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

    private void OnDestroy()
    {
        _inputActions.Disable();
    }

    public void FadeIn(List<FadeSetting> fadeSettings)
    {
        if (fadeSettings == null || fadeSettings.Count == 0)
        {
            Debug.LogWarning("Invalid input for FadeIn. Ensure the list is non-null and contains elements.");
            return;
        }

        foreach (var setting in fadeSettings)
        {
            if (setting.canvasGroup == null) continue;

            setting.canvasGroup.DOFade(1f, setting.duration).SetDelay(setting.delay).OnStart(() =>
            {
                if (setting.canvasGroup != null) // Check if CanvasGroup is still valid
                {
                    setting.canvasGroup.interactable = true;
                    setting.canvasGroup.blocksRaycasts = true;
                }
            });
        }
    }

    public void FadeOut(List<FadeSetting> fadeSettings)
    {
        if (fadeSettings == null || fadeSettings.Count == 0)
        {
            Debug.LogWarning("Invalid input for FadeOut. Ensure the list is non-null and contains elements.");
            return;
        }

        foreach (var setting in fadeSettings)
        {
            if (setting.canvasGroup == null) continue;

            setting.canvasGroup.DOFade(0f, setting.duration).SetDelay(setting.delay).OnComplete(() =>
            {
                if (setting.canvasGroup != null) // Check if CanvasGroup is still valid
                {
                    setting.canvasGroup.interactable = false;
                    setting.canvasGroup.blocksRaycasts = false;
                }
            });
        }
    }
}
