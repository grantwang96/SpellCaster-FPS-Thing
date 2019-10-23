using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class CharacterMoveController : MonoBehaviour { // Handles character movement

    [SerializeField] protected float _baseSpeed;
    [SerializeField] protected float _maxSpeed;
    [SerializeField] protected float _mass;
    [SerializeField] protected float _linearDrag;
    [SerializeField] protected float _headDistanceThreshold;
    [SerializeField] protected bool _performingAction;
    public bool performingAction { get { return _performingAction; } }

    protected CharacterController _characterController; // accesses the character controller on the character
    public CharacterController CharacterController { get { return _characterController; } }

    [SerializeField] protected Vector3 _movementVelocity;
    public Vector3 MovementVelocity { get { return _movementVelocity; } }

    protected Coroutine _busyAnimation; // coroutine that prevents other actions from being taken
    protected Coroutine _externalForces; // coroutine that prevents movement due to external forces

    protected virtual void Awake() {
        _characterController = GetComponent<CharacterController>();
    }

    protected virtual void Start() {

    }

    protected virtual void Update() {

    }

    protected virtual void FixedUpdate() {
        _movementVelocity = ProcessGravity(_movementVelocity);
        _characterController.Move(_movementVelocity * Time.deltaTime);
    }

    protected virtual Vector3 ProcessGravity(Vector3 vector) {
        if (vector.y > Physics.gravity.y) {
            vector.y += Time.deltaTime * Physics.gravity.y;
        }
        return vector;
    }

    public virtual void AddForce(Vector3 velocity, float drag) {
        if (!gameObject.activeInHierarchy) {
            return;
        }
        float linearDrag = drag > 0f ? drag : _linearDrag;
        if (_externalForces != null) {
            StopCoroutine(_externalForces);
        }
        _externalForces = StartCoroutine(ExternalForceRoutine(velocity, linearDrag));
    }

    protected virtual IEnumerator ExternalForceRoutine(Vector3 externalForce, float drag) {
        float linearDrag = drag;
        _movementVelocity += externalForce / _mass;
        _movementVelocity.y = externalForce.y;
        _characterController.Move(_movementVelocity * Time.deltaTime);
        yield return new WaitForFixedUpdate();
        while (!_characterController.isGrounded) {
            yield return new WaitForFixedUpdate();
        }
        float time = 0f;
        Vector3 start = _movementVelocity;
        while (_movementVelocity.x != 0 && _movementVelocity.z != 0) {
            time += Time.deltaTime * linearDrag;
            _movementVelocity.x = Mathf.Lerp(start.x, 0f, time);
            _movementVelocity.z = Mathf.Lerp(start.z, 0f, time);
            yield return new WaitForFixedUpdate();
        }
        _externalForces = null;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        // check if head was hit
        if (CollidedHead(hit.point) && !CharacterController.isGrounded &&
            (CharacterController.collisionFlags & CollisionFlags.Above) == 0) {
            _movementVelocity.y = 0f;
        }
    }

    private bool CollidedHead(Vector3 point) {
        Vector3 headPoint = transform.position + CharacterController.center + Vector3.up * CharacterController.height / 2f;
        return Vector3.Distance(headPoint, point) <= _headDistanceThreshold;
    }
}
