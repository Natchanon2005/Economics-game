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

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float sensitivity = 0.1f;
    private Vector3 _eulerAngles;      // เก็บมุมเริ่มต้นของกล้อง

    private OutlineManager currentOutline;
    private DepthOfField depthOfField;
    
    [SerializeField] private Volume globalVolume;
    [SerializeField] public bool allowCameraControl = true;
    [SerializeField] private Camera _camera;

    public void Initialize(Transform target)
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
        _eulerAngles = target.eulerAngles;
        transform.eulerAngles = _eulerAngles;
    }

    private void Start()
    {
        if (globalVolume != null && globalVolume.profile.TryGet(out depthOfField))
        {
            // DepthOfField retrieved successfully
        }
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
        if (allowCameraControl)
        {

            // ตรวจเช็ค Raycast สำหรับ Outline
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 5f) && hit.transform.CompareTag("Outline"))
            {
                OutlineManager om = hit.transform.GetComponent<OutlineManager>();

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
}
