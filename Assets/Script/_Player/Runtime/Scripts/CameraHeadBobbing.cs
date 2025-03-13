using UnityEngine;
using Unity.Cinemachine; // Changed from using Unity.Cinemachine;

public class CameraHeadBobbing : CinemachineExtension
{
    [SerializeField] private float bobFrequency = 1.5f;
    [SerializeField] private float bobAmplitude = 0.05f;
    [SerializeField] private float crouchBobAmplitude = 0.02f;
    [SerializeField] private float bobSpeedMultiplier = 0.3f;
    // New field for dynamic return speed
    [SerializeField] private float returnSpeed = 5f;

    private float _bobTimer;
    private float currentSpeed;
    private bool currentIsCrouching;
    // New smoothing field for bob offset
    private float currentBobOffset = 0f;

    private void Start()
    {
        Debug.Log("CameraHeadBobbing initialized.");
    }

    // This method will be called externally to update bobbing state
    public void UpdateHeadBobbing(float deltaTime, float speed, bool isCrouching)
    {
        currentSpeed = speed;
        currentIsCrouching = isCrouching;
        if (speed > 0.1f)
        {
            _bobTimer += deltaTime * bobFrequency * (speed * bobSpeedMultiplier);
        }
        else
        {
            _bobTimer = 0;
        }
    }

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        // Only apply during the Body stage
        if (stage == CinemachineCore.Stage.Body)
        {
            float amplitude = currentIsCrouching ? crouchBobAmplitude : bobAmplitude;
            float targetBobOffset = (currentSpeed > 0.1f) ? Mathf.Sin(_bobTimer) * amplitude : 0f;
            // Lerp dynamic bob offset for a smooth transition
            currentBobOffset = Mathf.Lerp(currentBobOffset, targetBobOffset, deltaTime * returnSpeed);
            state.PositionCorrection += new Vector3(0, currentBobOffset, 0);
        }
    }
}