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
    [SerializeField] protected float _linearDrag;
    [SerializeField] protected float _currentDrag;
    [SerializeField] protected float _headDistanceThreshold;

    [SerializeField] protected bool _performingAction;
    public bool performingAction { get { return _performingAction; } }

    protected CharacterController _characterController; // accesses the character controller on the character
    public CharacterController CharacterController { get { return _characterController; } }

    protected bool _hasControl = true;
    protected Vector3 _externalForce;
    [SerializeField] protected Vector3 _movementVelocity;
    public Vector3 MovementVelocity { get { return _movementVelocity; } }
    public bool IsGrounded => _characterController.isGrounded;

    protected Coroutine _busyAnimation; // coroutine that prevents other actions from being taken

    public event Action<float> OnMoveSpeedChanged;

    protected virtual void Awake() {
        _characterController = GetComponent<CharacterController>();
    }

    protected virtual void Start() {
        _currentDrag = _linearDrag;
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
        float linearDrag = drag > 0f ? drag : _currentDrag;
        _hasControl = allowControl;
        _currentDrag = drag;
        _externalForce += velocity / _mass;
    }

    public virtual void OverrideForce(Vector3 velocity, float drag, bool allowControl = false) {
        _externalForce = Vector3.zero;
        AddForce(velocity, drag, allowControl);
    }

    protected virtual void ProcessExternalForces() {
        _externalForce = ProcessGravity(_externalForce);
        if (!CharacterController.isGrounded) {
            return;
        }
        bool forced = false;
        if(Mathf.Approximately(0f, _externalForce.x)) {
            _externalForce.x = ExtraMath.ReduceAbsolute(_externalForce.x, Time.deltaTime * _currentDrag);
            forced = true;
        }
        if(Mathf.Approximately(0f, _externalForce.z)) {
            _externalForce.z = ExtraMath.ReduceAbsolute(_externalForce.z, Time.deltaTime * _currentDrag);
            forced = true;
        }
        if (!forced) {
            ResetMoveController();
        }
    }

    protected virtual void ResetMoveController() {
        _currentDrag = _linearDrag;
        _hasControl = true;
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
