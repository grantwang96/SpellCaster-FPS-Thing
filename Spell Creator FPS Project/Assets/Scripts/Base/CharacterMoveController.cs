using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class CharacterMoveController : MonoBehaviour { // Handles character movement

    [SerializeField] protected float baseSpeed;
    [SerializeField] protected bool _performingAction;
    public bool performingAction { get { return _performingAction; } }

    protected CharacterBehaviour characterBehaviour; // gain read access from character's brain
    protected CharacterController characterController; // accesses the character controller on the character

    protected Vector3 movementVelocity;

    protected Coroutine busyAnimation; // coroutine that prevents other actions from being taken

    public virtual void Awake() {
        characterBehaviour = GetComponent<CharacterBehaviour>();
        characterController = GetComponent<CharacterController>();
    }

    public virtual void Start() {

    }

    public virtual void Update() {

    }

    protected virtual void FixedUpdate() {
        ProcessGravity();
        characterController.Move(movementVelocity * Time.deltaTime);
        movementVelocity.x = 0f;
        movementVelocity.z = 0f;
    }

    protected virtual void ProcessGravity() {
        if (movementVelocity.y > Physics.gravity.y) {
            movementVelocity.y += Time.deltaTime * Physics.gravity.y;
        }
    }

    // move the character at this speed in this direction
    public virtual void Move(Vector3 moveDir, Vector3 target, float speed) {
        if (!performingAction) {
            moveDir = moveDir.normalized;
            movementVelocity.x = moveDir.x * speed;
            movementVelocity.z = moveDir.z * speed;

            SetRotation(target);
        }
    }

    // sets the rotation of the character
    public virtual void SetRotation(Vector3 target) {
        Vector3 lookDirection = target - transform.position;
        Vector3 lookDirectionBody = lookDirection;
        lookDirectionBody.y = 0;

        // TODO: set head to lookDirection
        characterBehaviour.Head.forward = lookDirection;

        // set root to lookDirectionBody;
        transform.forward = lookDirectionBody;
    }
}
