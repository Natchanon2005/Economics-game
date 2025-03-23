using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public struct CameraInput
{
    public Vector2 Look;
}

public enum DepthOfFieldState
{
    Enabled,
    Disabled
}

public enum LockState
{
    Lock,
    None
}
public enum VisibleState
{
    Show,
    Hidden
}

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private LockState lockState;
    [SerializeField] private VisibleState visibleState;

    public LockState LockState
    {
        get => lockState;
        set => lockState = value;
    }

    public VisibleState VisibleState
    {
        get => visibleState;
        set => visibleState = value;
    }

    [SerializeField] private float sensitivity = 0.1f;
    private Vector3 _eulerAngles;      // เก็บมุมเริ่มต้นของกล้อง

    private OutlineManager currentOutline;
    private DepthOfField depthOfField;
    
    [SerializeField] private Volume globalVolume;
    [SerializeField] public bool allowCameraControl = true;
    [SerializeField] private Camera _camera;
    [SerializeField] private UIManager uIManager;
    [SerializeField] private Customer currectCustomer;
    private PlayerInputActions _inputActions;
    private PlayerInputActions.UIActions _input;

    public void Initialize(Transform target)
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
        _eulerAngles = target.eulerAngles;
        transform.eulerAngles = _eulerAngles;
    }

    private void Start()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
        _input = _inputActions.UI;
    }

    public void SetDepthOfField(DepthOfFieldState state)
    {
        if (depthOfField != null)
        {
            depthOfField.active = state == DepthOfFieldState.Enabled;
        }
    }

    public void SetCameraControl(bool allow)
    {
        allowCameraControl = allow;
    }

    // ฟังก์ชันสำหรับรับค่า input ภายนอก (ถ้ามี)
    public void UpdateRotation(CameraInput input)
    {
        if (!allowCameraControl) return;
        _eulerAngles += new Vector3(-input.Look.y, input.Look.x) * sensitivity;
        transform.eulerAngles = _eulerAngles;
    }

    public void UpdatePosition(Transform target)
    {
        transform.position = target.position;
    }

    void Update()
    {
        MpuseState(lockState, visibleState);

        if (allowCameraControl)
        {

            // ตรวจเช็ค Raycast สำหรับ Outline
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 5f) && 
                (hit.transform.CompareTag("Outline") || 
                 hit.transform.CompareTag("Customer") || 
                 hit.transform.CompareTag("Product") ||
                 hit.transform.CompareTag("counter")))
            {
                OutlineManager om = hit.transform.GetComponent<OutlineManager>();
                if (hit.transform.CompareTag("Customer"))
                {
                    currectCustomer = hit.transform.GetComponent<Customer>();
                    if (_input.OpenOrder.triggered)
                    {
                        if (currectCustomer != null && !currectCustomer.HasBeenServed)
                        {
                            uIManager.ToggleOrderPanel();
                            currectCustomer.UpdateOrderText();
                            Debug.Log("OK");
                        }
                        else
                        {
                            Debug.LogWarning("Customer has already been served.");
                        }
                    }
                }

                if (hit.transform.CompareTag("Product"))
                {
                    if (_input.OpenOrder.triggered)
                    {
                        Product product = hit.transform.GetComponent<Product>();
                        product.AddProduct();
                    }
                }

                if (hit.transform.CompareTag("counter"))
                {
                    if (_input.OpenOrder.triggered)
                    {
                        uIManager.ToggleCoffeePanel();
                    }
                }

                if (om != null && om != currentOutline)
                {
                    // ถ้าเจอ OutlineManager ใหม่ ให้รีเซ็ทอันเก่า
                    ResetOutline();
                    currentOutline = om;
                    currentOutline.useDefaultSize = false;
                }
            }
            else
            {
                // ถ้าไม่เจออะไร ให้รีเซ็ทค่า defaultSize
                ResetOutline();
            }
        }
    }

    void ResetOutline()
    {
        if (currentOutline != null)
        {
            currentOutline.useDefaultSize = true;
            currentOutline = null;
        }
    }

    public void MpuseState(LockState state, VisibleState visible)
    {
        Cursor.lockState = state == LockState.Lock ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = visible == VisibleState.Show ? false : true;
    }

    public void Sell()
    {
        if (currectCustomer == null)
        {
            Debug.LogWarning("Sell failed: currectCustomer is null.");
            return;
        }

        if (uIManager == null || uIManager.Inventory == null || uIManager.ShopManager == null)
        {
            Debug.LogWarning("Sell failed: UIManager or its dependencies are null.");
            return;
        }

        if (uIManager.Inventory.SellDrink(currectCustomer.drinkName, uIManager.ShopManager.GetInflationMultiplier(), currectCustomer.amount)) 
        {
            uIManager.ToggleOrderPanel();
            currectCustomer.LeaveQueue();
            currectCustomer.HasBeenServed = true;
        }
    }
}
