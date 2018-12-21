using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class CharacterMoveController : MonoBehaviour { // Handles character movement

    [SerializeField] protected float _baseSpeed;
    [SerializeField] protected float _mass;
    [SerializeField] protected float _linearDrag;
    [SerializeField] protected bool _performingAction;
    public bool performingAction { get { return _performingAction; } }

    protected CharacterController characterController; // accesses the character controller on the character
    public CharacterController CharacterController { get { return characterController; } }

    [SerializeField] protected Vector3 movementVelocity;

    protected Coroutine busyAnimation; // coroutine that prevents other actions from being taken
    protected Coroutine externalForces; // coroutine that prevents movement due to external forces

    protected virtual void Awake() {
        characterController = GetComponent<CharacterController>();
    }

    protected virtual void Start() {

    }

    protected virtual void Update() {

    }

    protected virtual void FixedUpdate() {
        movementVelocity = ProcessGravity(movementVelocity);
        characterController.Move(movementVelocity * Time.deltaTime);
    }

    protected virtual Vector3 ProcessGravity(Vector3 vector) {
        if (vector.y > Physics.gravity.y) {
            vector.y += Time.deltaTime * Physics.gravity.y;
        }
        return vector;
    }

    public virtual void AddForce(Vector3 velocity) {
        if(externalForces != null) {
            StopCoroutine(externalForces);
        }
        externalForces = StartCoroutine(ExternalForceRoutine(velocity));
    }

    protected virtual IEnumerator ExternalForceRoutine(Vector3 externalForce) {
        movementVelocity = externalForce / _mass;
        float magnitude = externalForce.magnitude;
        characterController.Move(movementVelocity * Time.deltaTime);
        yield return new WaitForFixedUpdate();
        Vector3 start = externalForce;
        while (!characterController.isGrounded) {
            yield return null;
        }
        float time = 0f;
        while (movementVelocity.x != 0 && movementVelocity.z != 0) {
            time += Time.deltaTime * _linearDrag;
            movementVelocity.x = Mathf.Lerp(start.x, 0f, time);
            movementVelocity.z = Mathf.Lerp(start.z, 0f, time);
            yield return null;
        }
        externalForces = null;
    }
}
