using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private PlayerCamera playerCamera;
    [Space]
    [SerializeField] private CameraSpring cameraSpring;
    [SerializeField] private CameraLean cameraLean;

    private PlayerInputActions _inputActions;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();

        playerCharacter.Initialize();
        playerCamera.Initialize(playerCharacter.GetCameraTarget());

        cameraSpring.Initialize();
        cameraLean.Initialize();

        Debug.Log($"Player start position: {playerCharacter.transform.position}");
    }
    void Update()
    {

        var input = _inputActions.BasicInput;
        var deltaTime = Time.deltaTime;
        var cameraTarget = playerCharacter.GetCameraTarget();
        var state = playerCharacter.GetState();

        var cameraInput = new CameraInput { Look = input.Look.ReadValue<Vector2>() };
        playerCamera.UpdateRotation(cameraInput);
        playerCamera.UpdatePosition(cameraTarget);
        cameraSpring.UpdateSpring(deltaTime, cameraTarget.up);
        cameraLean.UpdateLean
        (
            deltaTime,
            state.Stance is Stance.Slide,
            state.Acceleration,
            cameraTarget.up
        );

        playerCamera.UpdateHeadBobbing(deltaTime, state.Velocity.magnitude, state.Stance == Stance.Crouch);

        var characterInput = new CharacterInput
        {
            Rotation    = playerCamera.transform.rotation,
            Move        = input.Move.ReadValue<Vector2>(),
            Run         = input.Run.IsPressed(),
            Jump        = input.Jump.WasPressedThisFrame(),
            JumpSustain = input.Jump.IsPressed(),
            Crouch      = input.Crouch.WasPressedThisFrame() 
                ? CrouchInput.Toggle 
                : CrouchInput.None,
        };
        playerCharacter.UpdateInput(characterInput);
        playerCharacter.UpdateBody(deltaTime);

        // Remove or comment out the debug teleport block to prevent unintended teleportation:
        /*
        #if UNITY_EDITOR
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            var ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out var hit))
            {
                Teleport(hit.point);
            }
        }
        #endif
        */
    }

    public void Teleport(Vector3 position)
    {
        playerCharacter.SetPosition(position);
    }

    public void SetMovementEnabled(bool enabled)
    {
        playerCharacter.SetMovementEnabled(enabled);
    }

    public void SetJumpEnabled(bool enabled)
    {
        playerCharacter.SetJumpEnabled(enabled);
    }

    public void SetCrouchEnabled(bool enabled)
    {
        playerCharacter.SetCrouchEnabled(enabled);
    }

    public void SetSlideEnabled(bool enabled)
    {
        playerCharacter.SetSlideEnabled(enabled);
    }
}
