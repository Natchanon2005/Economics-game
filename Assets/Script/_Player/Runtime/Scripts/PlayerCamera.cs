using Unity.Cinemachine;
using UnityEngine;

public struct CameraInput
{
    public Vector2 Look;
}

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float sensitivity = 0.1f;
    private Vector3 _eulerAngles;
    
    // New field for current Outline
    private Outline currentOutline;

    public void Initialize(Transform target)
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
        _eulerAngles = target.eulerAngles;
        transform.eulerAngles = _eulerAngles;
    }

    public void UpdateRotation(CameraInput input)
    {
        _eulerAngles += new Vector3(-input.Look.y, input.Look.x) * sensitivity;
        transform.eulerAngles = _eulerAngles;
    }

    public void UpdatePosition(Transform target)
    {
        transform.position = target.position;
    }
    
    // New method to update outline based on camera view
    void Update()
    {
        // Cast a ray from camera's position in its forward direction
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit,  5f))
        {
            Outline outline = hit.transform.GetComponent<Outline>();
            if (outline != null)
            {
                // If a new Outline is hit, disable the previous one
                if (currentOutline != outline)
                {
                    if (currentOutline != null)
                    {
                        currentOutline.enabled = false;
                    }
                    currentOutline = outline;
                }
                currentOutline.enabled = true;
            }
            else
            {
                if (currentOutline != null)
                {
                    currentOutline.enabled = false;
                    currentOutline = null;
                }
            }
        }
        else
        {
            if (currentOutline != null)
            {
                currentOutline.enabled = false;
                currentOutline = null;
            }
        }
    }
}