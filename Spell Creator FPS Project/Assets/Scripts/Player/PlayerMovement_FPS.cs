using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement_FPS : CharacterMoveController {
    
    [SerializeField] private float lookSpeed;
    [SerializeField] private float jumpForce;

    private bool _active;

    private void OnEnable() {
        GameplayController.Instance.OnJumpPressed += OnJump;
        GameplayController.Instance.OnControllerStateUpdated += OnControllerStateUpdated;
    }

    private void OnDisable() {
        GameplayController.Instance.OnJumpPressed -= OnJump;
        GameplayController.Instance.OnControllerStateUpdated -= OnControllerStateUpdated;
    }

    protected override void Start() {
        base.Start();
        OnControllerStateUpdated();
    }

    // Update is called once per frame
    protected override void Update () {
        _movementVelocity = ProcessGravity(_movementVelocity);
        if(_externalForces == null) {
            ProcessWalkInput();
        }
	}

    protected override void FixedUpdate() {
        _characterController.Move(_movementVelocity * Time.deltaTime);
    }

    private void ProcessWalkInput() {
        if (!_active) {
            _movementVelocity = Vector2.zero;
            return;
        }
        Vector3 inputVector = GameplayController.Instance.MoveVector;
        Vector3 newMoveVector = new Vector3();
        newMoveVector += transform.right * inputVector.x;
        newMoveVector += transform.forward * inputVector.z;
        _movementVelocity.x = newMoveVector.x * _baseSpeed;
        _movementVelocity.z = newMoveVector.z * _baseSpeed;
    }

    private void OnJump() {
        if (_characterController.isGrounded && _active) {
            _movementVelocity.y = jumpForce;
        }
    }

    protected override IEnumerator ExternalForceRoutine(Vector3 externalForce, float drag) {
        _movementVelocity = externalForce;
        yield return new WaitForEndOfFrame();
        Vector3 start = externalForce;
        while (!_characterController.isGrounded && (_characterController.velocity.x != 0 || _characterController.velocity.z != 0)) {
            yield return new WaitForFixedUpdate();
        }
        float time = 0f;
        while(_movementVelocity.x != 0 && _movementVelocity.z != 0) {
            time += Time.deltaTime * drag;
            Vector3 yVel = new Vector3(0f, _movementVelocity.y, 0f);
            _movementVelocity = Vector3.Lerp(start, yVel, time);
            // _movementVelocity.x = Mathf.Lerp(start.x, 0f, time);
            // _movementVelocity.z = Mathf.Lerp(start.z, 0f, time);
            yield return new WaitForFixedUpdate();
        }
        _externalForces = null;
    }

    private void OnControllerStateUpdated() {
        _active = GameplayController.Instance.ControllerState == ControllerState.Gameplay;
    }
}
