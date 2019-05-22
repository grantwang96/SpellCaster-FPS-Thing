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
    public ControlScheme controlScheme => _controlScheme;
    [SerializeField] private bool _uiPanelsEmpty;

    [SerializeField] protected Vector2 _lookVector; // Vector that saves camera controls input
    public Vector2 LookVector { get { return _lookVector; } }
    public override Vector3 GetBodyPosition() {
        return _bodyTransform.position + _playerMovement.CharacterController.center;
    }

    public delegate void BasicBtnEvent(); // for button presses that don't need to pass information
    public event BasicBtnEvent OnJumpPressed;

    public event BasicBtnEvent OnInteractPressed;
    public event BasicBtnEvent OnInteractHeld;
    public event BasicBtnEvent OnInteractReleased;

    public event BasicBtnEvent OnFire1Pressed;
    public event BasicBtnEvent OnFire1Held;
    public event BasicBtnEvent OnFire1End;

    private PlayerMovement_FPS _playerMovement; // component that moves the player
    private PlayerCamera_FPS _playerCamera; // component that controls the camera
    private PlayerCombat _playerCombat;
    public ISpellCaster PlayerCombat => _playerCombat;
    private PlayerDamageable _playerDamageable;
    public override Damageable Damageable => _playerDamageable;

    [SerializeField] private UIPanel _menuPrefab;

    protected override void Awake() {
        Instance = this;
        base.Awake();
    }

    // Use this for initialization
    void Start () {
        SetMouseEnabled(false);
        UIManager.Instance.OnPanelsUpdated += OnUIPanelsUpdated;
        InitializeComponents();
	}
	
	// Update is called once per frame
	void Update () {
        if (!_uiPanelsEmpty) { return; }
        ProcessInputs();
	}

    public override float GetMoveMagnitude() {
        Vector3 vel = _playerMovement.CharacterController.velocity;
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
        _playerMovement = GetComponent<PlayerMovement_FPS>();
        _playerCamera = GetComponent<PlayerCamera_FPS>();
        _playerCamera.Head = Head;
        _playerCamera.Body = transform;
        _playerCombat = GetComponent<PlayerCombat>();
        _playerDamageable = GetComponent<PlayerDamageable>();
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
        _moveVector.x = Input.GetAxis("Horizontal");
        _moveVector.z = Input.GetAxis("Vertical");

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

        if (Input.GetButtonDown("Cancel")) {
            MenuPressed();
        }
    }

    private void Jump() {
        OnJumpPressed?.Invoke();
    }

    private void InteractPressed() {
        OnInteractPressed?.Invoke();
    }

    private void InteractHeld() {
        OnInteractHeld?.Invoke();
    }

    private void InteractReleased() {
        OnInteractReleased?.Invoke();
    }

    private void Shoot1Pressed() {
        OnFire1Pressed?.Invoke();
    }

    private void Shoot1Held() {
        OnFire1Held?.Invoke();
    }

    private void Shoot1Released() {
        OnFire1End?.Invoke();
    }

    private void MenuPressed() {
        UIManager.Instance.OpenUIPanel(_menuPrefab);
    }

    // event that is called when UIManager updates its panels
    private void OnUIPanelsUpdated(bool panelsEmpty) {
        _uiPanelsEmpty = panelsEmpty;
        Cursor.lockState = panelsEmpty ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !panelsEmpty;
        _moveVector.x = panelsEmpty ? _moveVector.x : 0f;
        _moveVector.z = panelsEmpty ? _moveVector.z : 0f;
        _lookVector.x = panelsEmpty ? _lookVector.x : 0f;
        _lookVector.y = panelsEmpty ? _lookVector.y : 0f;
    }
}
