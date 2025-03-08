using UnityEngine;

public struct CameraInput
{
    public Vector2 Look;
}

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float sensitivity = 0.1f;
    [SerializeField] private float bobFrequency = 1.5f;
    [SerializeField] private float bobAmplitude = 0.05f;
    [SerializeField] private float crouchBobAmplitude = 0.02f;
    [SerializeField] private float bobSpeedMultiplier = 0.3f;

    private Vector3 _eulerAngles;
    private float _bobTimer;
    private Transform _target;

    public void Initialize(Transform target)
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
        transform.eulerAngles = _eulerAngles = target.eulerAngles;
        _target = target;

        Debug.Log($"Player start position: {gameObject.transform.position}");
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

    public void UpdateHeadBobbing(float deltaTime, float speed, bool isCrouching)
    {
        if (speed > 0.1f)
        {
            _bobTimer += deltaTime * bobFrequency * (speed * bobSpeedMultiplier);
            float amplitude = isCrouching ? crouchBobAmplitude : bobAmplitude;
            float bobOffset = Mathf.Sin(_bobTimer) * amplitude;
            transform.localPosition = new Vector3(transform.localPosition.x, _target.localPosition.y + bobOffset, transform.localPosition.z);
        }
        else
        {
            _bobTimer = 0;
            transform.localPosition = new Vector3(transform.localPosition.x, _target.localPosition.y, transform.localPosition.z);
        }
    }
}
