using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class CharacterMoveController : MonoBehaviour { // Handles character movement

    [SerializeField] protected float _baseSpeed;
    public float BaseSpeed => _baseSpeed;
    [SerializeField] protected float _maxSpeed;
    public float MaxSpeed => _maxSpeed;

    [SerializeField] protected float _moveSpeed;
    public float MoveSpeed {
        get {
            return _moveSpeed;
        }
        protected set {
            _moveSpeed = value;
            OnMoveSpeedChanged?.Invoke(_moveSpeed);
        }
    }
    [SerializeField] protected float _mass;

    [SerializeField] protected float _airDrag;
    [SerializeField] protected float _groundDrag;
    [SerializeField] protected float _currentDrag;

    [SerializeField] protected float _headDistanceThreshold;

    [SerializeField] protected bool _performingAction;
    public bool performingAction { get { return _performingAction; } }

    protected CharacterController _characterController; // accesses the character controller on the character
    public CharacterController CharacterController { get { return _characterController; } }

    protected bool _hasControl = true;
    [SerializeField] protected Vector3 _externalForce;
    [SerializeField] protected Vector3 _movementVelocity;
    public Vector3 MovementVelocity { get { return _movementVelocity; } }
    public bool IsGrounded => _characterController.isGrounded;

    protected Coroutine _busyAnimation; // coroutine that prevents other actions from being taken

    public event Action<float> OnMoveSpeedChanged;

    protected virtual void Awake() {
        _characterController = GetComponent<CharacterController>();
    }

    protected virtual void Start() {
        _currentDrag = _groundDrag;
    }

    protected virtual void Update() {

    }

    protected virtual void FixedUpdate() {
        // _externalForce = ProcessGravity(_externalForce);
        ProcessExternalForces();
        ProcessMovement();
    }

    protected virtual void ProcessMovement() {
        if (!_hasControl) {
            _movementVelocity = Vector3.zero;
        }
        Vector3 move = _movementVelocity + _externalForce;
        _characterController.Move(move * Time.deltaTime);
    }

    protected virtual Vector3 ProcessGravity(Vector3 vector) {
        if (vector.y > Physics.gravity.y) {
            vector.y += Time.deltaTime * Physics.gravity.y * _currentDrag;
        }
        return vector;
    }

    public virtual void AddForce(Vector3 velocity, float drag, bool allowControl = false) {
        if (!gameObject.activeInHierarchy) {
            return;
        }
        _currentDrag = GetDrag(drag, velocity.y);
        _externalForce += velocity / _mass;
        _hasControl = allowControl;
    }

    protected float GetDrag(float overrideDrag, float upwardForce) {
        if(overrideDrag > 0f) {
            return overrideDrag;
        }
        return (upwardForce > 0f || !_characterController.isGrounded) ? _airDrag : _groundDrag;
    }

    public virtual void OverrideForce(Vector3 velocity, float drag, bool allowControl = false) {
        _externalForce = Vector3.zero;
        AddForce(velocity, drag, allowControl);
    }

    protected virtual void ProcessExternalForces() {
        _externalForce = ProcessGravity(_externalForce);
        ReduceExternalForce();
    }

    protected void ReduceExternalForce() {
        if (Mathf.Approximately(_externalForce.x, 0f) && Mathf.Approximately(_externalForce.z, 0f)) {
            _currentDrag = GetDrag(0f, _externalForce.y);
            _hasControl = true;
            return;
        }
        float vertical = _externalForce.y;
        Vector3 lateralExternalForce = _externalForce;
        lateralExternalForce.y = 0f;
        _externalForce = ExtraMath.ReduceVector(lateralExternalForce, Time.deltaTime * _currentDrag);
        _externalForce.y = vertical;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        // check if head was hit
        if (CollidedHead(hit.point) && !CharacterController.isGrounded &&
            (CharacterController.collisionFlags & CollisionFlags.Above) == 0) {
            _externalForce.y = 0f;
        }
    }

    private bool CollidedHead(Vector3 point) {
        Vector3 headPoint = transform.position + CharacterController.center + Vector3.up * CharacterController.height / 2f;
        return Vector3.Distance(headPoint, point) <= _headDistanceThreshold;
    }
}
