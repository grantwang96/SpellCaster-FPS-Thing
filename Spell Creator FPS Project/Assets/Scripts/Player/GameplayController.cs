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
    public Vector2 lookVector { get { return _lookVector; } }

    private PlayerMovement_FPS playerMovement; // component that moves the player
    private PlayerCamera_FPS playerCamera; // component that controls the camera

    protected override void Awake() {
        Instance = this;
        base.Awake();
    }

    // Use this for initialization
    void Start () {
        HideAndLockMouse();
        InitializeComponents();
	}
	
	// Update is called once per frame
	void Update () {
        ProcessInputs();
        ApplyAxisInputs();
	}

    // hides the mouse
    private void HideAndLockMouse() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void InitializeComponents() {
        playerMovement = GetComponent<PlayerMovement_FPS>();
        playerCamera = GetComponent<PlayerCamera_FPS>();
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

        if (Input.GetButtonDown("Interact")) {  }
        else if (Input.GetButtonUp("Interact")) {  }

        if (Input.GetButtonDown("Fire1")) { Shoot(); }
    }

    private void ApplyAxisInputs() {

    }

    private void ApplyButtonInputs() {

    }

    private void Jump() {

    }

    private void InteractDown() {

    }

    private void InteractUp() {

    }

    private void Shoot() {

    }
}
