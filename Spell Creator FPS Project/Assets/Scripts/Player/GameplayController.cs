using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControllerState {
    Gameplay, // normal gameplay state
    UIMenu, // UI menu state
}

/// <summary>
/// This is the in game controller that processes player's inputs and applies to other components
/// </summary>
public class GameplayController : CharacterBehaviour {

    public static GameplayController Instance { get; private set; }

    public enum ControlScheme {
        MouseKeyboard, Controller
    }
    [SerializeField] private ControlScheme _controlScheme;
    public ControlScheme controlScheme => _controlScheme;
    [SerializeField] private bool _uiPanelsEmpty;

    public ControllerState ControllerState { get; private set; }
    public event Action OnControllerStateUpdated;

    private Vector2 _moveVectorRaw = new Vector2();
    public event Action<Vector2> DirectionalInput;

    private float _holdTime;
    private float _intervalHoldTime;

    [Range(0f, 1f)] [SerializeField] private float _directionalHoldThreshold; // how long before auto moving
    [Range(0f, 1f)] [SerializeField] private float _directionalHoldFreq; // how quickly it should move
    private bool _directionalButtonPressed;

    [SerializeField] protected Vector2 _lookVector; // Vector that saves camera controls input
    public Vector2 LookVector { get { return _lookVector; } }
    public override Vector3 GetBodyPosition() {
        return _bodyTransform.position + _playerMovement.CharacterController.center;
    }

    public event Action OnJumpPressed;

    public event Action OnInteractPressed;
    public event Action OnInteractHeld;
    public event Action OnInteractReleased;

    public event Action OnFire1Pressed;
    public event Action OnFire1Held;
    public event Action OnFire1Released;

    public event Action OnFire2Pressed;
    public event Action OnFire2Held;
    public event Action OnFire2Released;

    public event Action OnSubmitPressed;
    public event Action OnSubmitHeld;
    public event Action OnSubmitReleased;

    public event Action OnCancelPressed;

    public event Action<int> OnSlotBtnPressed;

    private PlayerMovement_FPS _playerMovement; // component that moves the player
    private PlayerCamera_FPS _playerCamera; // component that controls the camera
    private PlayerCombat _playerCombat;
    public ISpellCaster PlayerCombat => _playerCombat;
    private PlayerDamageable _playerDamageable;
    public override Damageable Damageable => _playerDamageable;

    protected override void Awake() {
        Instance = this;
        base.Awake();
    }

    // Use this for initialization
    void Start() {
        SetMouseEnabled(false);
        UIManager.Instance.OnPanelsUpdated += OnUIPanelsUpdated;
        InitializeComponents();
    }

    // Update is called once per frame
    void Update() {
        // do not run if UI is active
        // if (!_uiPanelsEmpty) { return; }
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

        _moveVectorRaw.x = Input.GetAxisRaw("Horizontal");
        _moveVectorRaw.y = Input.GetAxisRaw("Vertical");

        UpdateDirectionalInputData();

        _lookVector.x = Input.GetAxis("Mouse X");
        _lookVector.y = Input.GetAxis("Mouse Y");

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            OnSlotBtnPressed?.Invoke(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            OnSlotBtnPressed?.Invoke(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            OnSlotBtnPressed?.Invoke(3);
        }
    }

    private void ControllerInputs() {

    }

    private void UpdateDirectionalInputData() {
        bool tempDirectionalButtonPressed = _moveVectorRaw.x != 0f || _moveVectorRaw.y != 0f;
        if (tempDirectionalButtonPressed) {
            // if initial press of button
            if (!_directionalButtonPressed) {
                _holdTime = 0f;
                _intervalHoldTime = 0f;
                _directionalButtonPressed = tempDirectionalButtonPressed;
                DirectionalInput?.Invoke(_moveVectorRaw);
                return;
            }
            if (_holdTime <= _directionalHoldThreshold) {
                _holdTime += Time.deltaTime;
            } else if (_intervalHoldTime >= _directionalHoldFreq) {
                _intervalHoldTime = 0f;
                DirectionalInput?.Invoke(_moveVectorRaw);
                return;
            }
            _intervalHoldTime += Time.deltaTime;
        }
        _directionalButtonPressed = tempDirectionalButtonPressed;
    }

    private void GetButtonInputs() {
        
        if (Input.GetButtonDown("Jump")) { Jump(); }

        if (Input.GetButtonDown("Gameplay_Interact")) { InteractPressed(); }
        else if (Input.GetButton("Gameplay_Interact")) { InteractHeld(); }
        else if (Input.GetButtonUp("Gameplay_Interact")) { InteractReleased(); }

        if (Input.GetButtonDown("Fire1")) { Shoot1Pressed(); }
        else if (Input.GetButton("Fire1")) { Shoot1Held(); }
        else if (Input.GetButtonUp("Fire1")) { Shoot1Released(); }

        if (Input.GetButtonDown("Submit")) { SubmitPressed(); }

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
        OnFire1Released?.Invoke();
    }

    private void Shoot2Pressed() {
        OnFire2Pressed?.Invoke();
    }

    private void Shoot2Held() {
        OnFire2Held?.Invoke();
    }

    private void Shoot2Released() {
        OnFire2Released?.Invoke();
    }

    private void SubmitPressed() {
        OnSubmitPressed?.Invoke();
    }

    private void SubmitHeld() {
        OnSubmitHeld?.Invoke();
    }

    private void SubmitReleased() {
        OnSubmitReleased.Invoke();
    }

    private void MenuPressed() {
        OnCancelPressed?.Invoke();
    }

    // event that is called when UIManager updates its panels
    private void OnUIPanelsUpdated(bool panelsActive) {
        _uiPanelsEmpty = !panelsActive;
        SetMouseEnabled(!_uiPanelsEmpty);
        _moveVector.x = _uiPanelsEmpty ? _moveVector.x : 0f;
        _moveVector.z = _uiPanelsEmpty ? _moveVector.z : 0f;
        _lookVector.x = _uiPanelsEmpty ? _lookVector.x : 0f;
        _lookVector.y = _uiPanelsEmpty ? _lookVector.y : 0f;

        ControllerState = _uiPanelsEmpty ? ControllerState.Gameplay : ControllerState.UIMenu;
        OnControllerStateUpdated?.Invoke();
    }
}
