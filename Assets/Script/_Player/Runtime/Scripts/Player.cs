using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] public float money = 0;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private PlayerCamera playerCamera;
    [Space]
    [SerializeField] private CameraSpring cameraSpring;
    [SerializeField] private CameraLean cameraLean;
    // Added field for head bobbing integration
    [SerializeField] private CameraHeadBobbing cameraHeadBobbing;

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

        // Update head bobbing: use player's velocity magnitude as speed
        // and consider crouching if in Crouch or Slide stance
        bool isCrouching = state.Stance == Stance.Crouch || state.Stance == Stance.Slide;
        cameraHeadBobbing.UpdateHeadBobbing(Time.deltaTime, state.Velocity.magnitude, isCrouching);

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
        UpdateMoney();
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

    public void BuyItem(float cost)
    {
        if (money >= cost)
        {
            money -= cost;
            Debug.Log($"Item bought for {cost}. Remaining money: {money}");
        }
        else
        {
            Debug.Log("Not enough money to buy item.");
        }
    }

    public void UpdateMoney()
    {
        moneyText.text = money + " บาท";
    }
}
