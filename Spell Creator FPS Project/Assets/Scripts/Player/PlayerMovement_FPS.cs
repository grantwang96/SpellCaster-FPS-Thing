using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement_FPS : CharacterMoveController {
    
    [SerializeField] private float lookSpeed;
    [SerializeField] private float jumpForce;

    [SerializeField] private bool _active;
    
    private void OnDisable() {
        UnsubscribeToController();
    }

    private void SubscribeToController() {
        PlayerController.Instance.OnJumpPressed += OnJump;
        PlayerController.Instance.Damageable.OnDeath += OnPlayerDeath;
        PlayerController.Instance.OnControllerStateUpdated += OnControllerStateUpdated;

    }

    private void UnsubscribeToController() {
        PlayerController.Instance.OnJumpPressed -= OnJump;
        PlayerController.Instance.Damageable.OnDeath -= OnPlayerDeath;
        PlayerController.Instance.OnControllerStateUpdated -= OnControllerStateUpdated;

    }

    protected override void Start() {
        base.Start();
        SubscribeToController();
        OnControllerStateUpdated();
    }

    // Update is called once per frame
    protected override void Update () {
        ProcessWalkInput();
    }
    
    private void ProcessWalkInput() {
        if (!_active) {
            _movementVelocity = Vector2.zero;
            return;
        }
        Vector3 inputVector = PlayerController.Instance.MoveVector;
        Vector3 newMoveVector = new Vector3();
        newMoveVector += transform.right * inputVector.x;
        newMoveVector += transform.forward * inputVector.z;
        _movementVelocity.x = newMoveVector.x * _baseSpeed;
        _movementVelocity.z = newMoveVector.z * _baseSpeed;
    }

    private void OnJump() {
        if (_characterController.isGrounded && _active) {
            _externalForce.y = jumpForce;
        }
    }
    
    protected override IEnumerator ExternalForceRoutine(Vector3 externalForce, float drag) {
        _externalForce = externalForce;
        yield return new WaitForEndOfFrame();
        while (!_characterController.isGrounded) {
            yield return new WaitForFixedUpdate();
        }
        Vector3 start = _externalForce;
        float time = 0f;
        while(time < 1f) {
            _externalForce.x = Mathf.Lerp(start.x, 0f, time);
            _externalForce.z = Mathf.Lerp(start.z, 0f, time);
            time += Time.deltaTime * drag;
            yield return new WaitForFixedUpdate();
        }
        _externalForce = Vector3.zero;
        _externalForces = null;
    }

    private void OnControllerStateUpdated() {
        _active = PlayerController.Instance.ControllerState == ControllerState.Gameplay;
    }

    private void OnPlayerDeath(bool isDead, Damageable damageable) {
        _active = false;
        UnsubscribeToController();
    }
}
