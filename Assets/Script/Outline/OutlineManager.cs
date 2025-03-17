using UnityEngine;

public class OutlineManager : MonoBehaviour
{
    [SerializeField] public bool useDefaultSize = true; // Toggle เลือกขนาด default หรือ custom
    [Space]
    public Renderer targetRenderer;  
    [Range(0f, 2f)]
    [SerializeField] public float sizeValue = 1.03f;
    [SerializeField] private float defaultSizeValue = 1f;
    [SerializeField] public float smoothSpeed = 7f;

    [SerializeField] private Material targetMaterial;
    private int sizeID;

    void Start()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        // เช็คว่า Renderer มี material ตำแหน่งที่สอง
        if (targetRenderer != null && targetRenderer.materials.Length > 1)
        {
            targetMaterial = targetRenderer.materials[1];
            sizeID = Shader.PropertyToID("_Size"); // Cache property ID
            targetMaterial.SetFloat(sizeID, defaultSizeValue);
        }
        else
        {
            Debug.LogError("OutlineManager: Target material is not properly set up.");
            return;
        }
    }

    void Update()
    {
        if (targetMaterial == null) return;

        // เลือกขนาดว่าจะใช้ default หรือ custom ตาม toggle
        float targetSize = useDefaultSize ? defaultSizeValue : sizeValue;
        float currentSize = targetMaterial.GetFloat(sizeID);
        float newSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * smoothSpeed);
        
        targetMaterial.SetFloat(sizeID, newSize);
    }
}
