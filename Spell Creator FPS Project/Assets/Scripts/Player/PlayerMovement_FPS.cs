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
        movementVelocity = ProcessGravity(movementVelocity);
        if(externalForces == null) {
            ProcessWalkInput();
        }
	}

    protected override void FixedUpdate() {
        characterController.Move(movementVelocity * Time.deltaTime);
    }

    private void ProcessWalkInput() {
        Vector3 inputVector = GameplayController.Instance.walkVector;
        Vector3 newMoveVector = new Vector3();
        newMoveVector += transform.right * inputVector.x;
        newMoveVector += transform.forward * inputVector.z;
        movementVelocity.x = newMoveVector.x * _baseSpeed;
        movementVelocity.z = newMoveVector.z * _baseSpeed;
    }

    private void OnJump() {
        if (characterController.isGrounded) {
            movementVelocity.y = jumpForce;
        }
    }

    protected override IEnumerator ExternalForceRoutine(Vector3 externalForce) {
        movementVelocity = externalForce;
        yield return new WaitForEndOfFrame();
        Vector3 start = externalForce;
        while (!characterController.isGrounded && (characterController.velocity.x != 0 || characterController.velocity.z != 0)) {
            yield return null;
        }
        float time = 0f;
        while(movementVelocity.x != 0 && movementVelocity.z != 0) {
            time += Time.deltaTime;
            movementVelocity.x = Mathf.Lerp(start.x, 0f, time);
            movementVelocity.z = Mathf.Lerp(start.z, 0f, time);
            yield return null;
        }
        externalForces = null;
    }
}
