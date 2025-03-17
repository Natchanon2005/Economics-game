using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private UITap objectToDisplay;
    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private PlayerCharacter playerCharacter;

    private PlayerInputActions inputActions;
    private bool isObjectVisible = false;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        SubscribeToInputActions();
    }

    private void OnDisable()
    {
        UnsubscribeFromInputActions();
    }

    private void SubscribeToInputActions()
    {
        inputActions.UI.OpenStore.performed += ToggleDisplay;
        inputActions.UI.Enable();
    }

    private void UnsubscribeFromInputActions()
    {
        inputActions.UI.OpenStore.performed -= ToggleDisplay;
        inputActions.UI.Disable();
    }

    private void ToggleDisplay(InputAction.CallbackContext context)
    {
        isObjectVisible = !isObjectVisible;
        UpdateUIVisibility();
    }

    private void UpdateUIVisibility()
    {
        playerCharacter.movementEnabled = !isObjectVisible;
        playerCharacter.jumpEnabled = !isObjectVisible;
        playerCamera.allowCameraControl = !isObjectVisible;
        Cursor.visible = isObjectVisible;
        Cursor.lockState = isObjectVisible ? CursorLockMode.None : CursorLockMode.Locked;
        objectToDisplay.gameObject.SetActive(isObjectVisible);
        playerCamera.SetDepthOfField(isObjectVisible ? DepthOfFieldState.Enabled : DepthOfFieldState.Disabled);
        objectToDisplay.enabled = isObjectVisible;
    }
}
