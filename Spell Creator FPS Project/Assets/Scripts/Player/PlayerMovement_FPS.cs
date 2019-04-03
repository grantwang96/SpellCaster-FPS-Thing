using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement_FPS : CharacterMoveController {
    
    [SerializeField] private float lookSpeed;
    [SerializeField] private float jumpForce;

    private void OnEnable() {
        GameplayController.Instance.OnJumpPressed += OnJump;
    }

    private void OnDisable() {
        GameplayController.Instance.OnJumpPressed -= OnJump;
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
        Vector3 inputVector = GameplayController.Instance.MoveVector;
        Vector3 newMoveVector = new Vector3();
        newMoveVector += transform.right * inputVector.x;
        newMoveVector += transform.forward * inputVector.z;
        _movementVelocity.x = newMoveVector.x * _baseSpeed;
        _movementVelocity.z = newMoveVector.z * _baseSpeed;
    }

    private void OnJump() {
        if (_characterController.isGrounded) {
            _movementVelocity.y = jumpForce;
        }
    }

    protected override IEnumerator ExternalForceRoutine(Vector3 externalForce) {
        _movementVelocity = externalForce;
        yield return new WaitForEndOfFrame();
        Vector3 start = externalForce;
        while (!_characterController.isGrounded && (_characterController.velocity.x != 0 || _characterController.velocity.z != 0)) {
            yield return null;
        }
        float time = 0f;
        while(_movementVelocity.x != 0 && _movementVelocity.z != 0) {
            time += Time.deltaTime;
            _movementVelocity.x = Mathf.Lerp(start.x, 0f, time);
            _movementVelocity.z = Mathf.Lerp(start.z, 0f, time);
            yield return null;
        }
        _externalForces = null;
    }
}
