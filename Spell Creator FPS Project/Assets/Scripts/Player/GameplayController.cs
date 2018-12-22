using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the in game controller that processes player's inputs and applies to other components
/// </summary>
public class GameplayController : CharacterBehaviour {

    public static GameplayController Instance;

    public enum ControlScheme {
        MouseKeyboard, Controller
    }
    [SerializeField] private ControlScheme _controlScheme;

    [SerializeField] protected Vector2 _lookVector; // Vector that saves camera controls input
    public Vector2 LookVector { get { return _lookVector; } }
    public override Vector3 GetBodyPosition() {
        return _bodyTransform.position + playerMovement.CharacterController.center;
    }

    public delegate void BasicBtnEvent(); // for button presses that don't need to pass information
    public event BasicBtnEvent OnJumpPressed;

    public event BasicBtnEvent OnInteractPressed;
    public event BasicBtnEvent OnInteractHeld;
    public event BasicBtnEvent OnInteractReleased;

    public event BasicBtnEvent OnFire1Pressed;
    public event BasicBtnEvent OnFire1Held;
    public event BasicBtnEvent OnFire1End;

    private PlayerMovement_FPS playerMovement; // component that moves the player
    private PlayerCamera_FPS playerCamera; // component that controls the camera
    private PlayerCombat playerCombat;

    protected override void Awake() {
        Instance = this;
        base.Awake();
    }

    // Use this for initialization
    void Start () {
        SetMouseEnabled(false);
        InitializeComponents();
	}
	
	// Update is called once per frame
	void Update () {
        ProcessInputs();
	}

    public override float GetMoveMagnitude() {
        Vector3 vel = playerMovement.CharacterController.velocity;
        vel.y = 0f;
        return vel.magnitude;
    }

    // hides the mouse
    private void SetMouseEnabled(bool enabled) {
        if (enabled) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void InitializeComponents() {
        playerMovement = GetComponent<PlayerMovement_FPS>();
        playerCamera = GetComponent<PlayerCamera_FPS>();
        playerCamera.Head = Head;
        playerCamera.Body = transform;
        playerCombat = GetComponent<PlayerCombat>();
    }

    private void ProcessInputs() {
        GetAxisInputs();
        GetButtonInputs();
    }

    private void GetAxisInputs() {
        switch (_controlScheme) {
            case ControlScheme.MouseKeyboard:
                MouseKeyboardInputs();
                break;
            case ControlScheme.Controller:
                ControllerInputs();
                break;
            default:
                break;
        }
    }

    private void MouseKeyboardInputs() {
        _walkVector.x = Input.GetAxis("Horizontal");
        _walkVector.z = Input.GetAxis("Vertical");

        _lookVector.x = Input.GetAxis("Mouse X");
        _lookVector.y = Input.GetAxis("Mouse Y");
    }

    private void ControllerInputs() {

    }

    private void GetButtonInputs() {
        
        if (Input.GetButtonDown("Jump")) { Jump(); }

        if (Input.GetButtonDown("Interact")) { InteractPressed(); }
        else if (Input.GetButton("Interact")) { InteractHeld(); }
        else if (Input.GetButtonUp("Interact")) { InteractReleased(); }

        if (Input.GetButtonDown("Fire1")) { Shoot1Pressed(); }
        else if (Input.GetButton("Fire1")) { Shoot1Held(); }
        else if (Input.GetButtonUp("Fire1")) { Shoot1Released(); }
    }

    private void Jump() {
        OnJumpPressed.Invoke();
    }

    private void InteractPressed() {
        if (OnInteractPressed == null) {
            Debug.LogError("Interact Pressed event is empty!");
            return;
        }
        OnInteractPressed.Invoke();
    }

    private void InteractHeld() {
        if (OnInteractHeld == null) {
            Debug.LogError("Interact Hold event is empty!");
            return;
        }
        OnInteractHeld.Invoke();
    }

    private void InteractReleased() {
        if(OnInteractReleased == null) {
            Debug.LogError("Interact Released event is empty!");
            return;
        }
        OnInteractReleased.Invoke();
    }

    private void Shoot1Pressed() {
        OnFire1Pressed.Invoke();
    }

    private void Shoot1Held() {
        OnFire1Held.Invoke();
    }

    private void Shoot1Released() {
        OnFire1End.Invoke();
    }
}
