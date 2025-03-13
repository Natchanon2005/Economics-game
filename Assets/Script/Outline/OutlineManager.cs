using UnityEngine;

public class OutlineManager : MonoBehaviour
{
    public bool look;
    [Space]
    [SerializeField] private Outline.Mode outlineMode;
    [SerializeField] private Color outlineColor;
    [SerializeField] private float outlineWidth;
    private Outline outline;

    void Start()
    {
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.OutlineMode = outlineMode;
            outline.OutlineColor = outlineColor;
            outline.OutlineWidth = outlineWidth;
            outline.enabled = false;
        }
    }
}