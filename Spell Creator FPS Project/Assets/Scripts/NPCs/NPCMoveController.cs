using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMoveController : CharacterMoveController {

    [SerializeField] protected float _turnSpeed;
    [SerializeField] protected float _destinationArrivalDistance; // distance NPC is considered "arrived" at destination

    protected NPCBehaviour _npcBehaviour; // gain read access from character's brain
    protected NPCVision _npcVision; // vision component
    protected NavMeshAgent _agent;
    
    [SerializeField] protected Vector3 _currentPathCorner; // what the NPC is moving towards(usually path corner)
    public Vector3 CurrentPathCorner { get { return _currentPathCorner; } }
    protected Vector3 _currentLookTarget; // what the NPC is looking towards
    public Vector3 CurrentLookTarget { get { return _currentLookTarget; } }

    [SerializeField] protected bool _traveling;
    public bool PathPending { get; private set; }

    public delegate void PathCalculatedDelegate(NavMeshPathStatus pathStatus);
    public event PathCalculatedDelegate OnPathCalculated; // called when path is finished calculating
    public delegate void ArrivedDestinationDelegate();
    public event ArrivedDestinationDelegate OnArrivedDestination;

    protected NavMeshPath _path;
    public Vector3[] Path { get; protected set; }
    [SerializeField] protected int _pathIndex;

    private bool NextPathCorner() {
        if (Path == null || Path.Length == 0) { return false; }
        if (_pathIndex + 1 >= Path.Length) { return false; }
        _pathIndex++;
        return true;
    }

    protected override void Awake() {
        base.Awake();
        _npcBehaviour = GetComponent<NPCBehaviour>();
        _npcVision = GetComponent<NPCVision>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.updatePosition = false;
        _agent.updateRotation = false;
        _path = new NavMeshPath();
    }

    protected override void Start() {
        base.Start();
        _baseSpeed = _npcBehaviour.Blueprint.WalkSpeed;
        _maxSpeed = _npcBehaviour.Blueprint.RunSpeed;
    }

    protected override void Update() {
        ProcessPathing();
        ProcessMovement();
    }

    // move character towards current destination if traveling
    protected override void ProcessMovement() {
        if (_traveling) {
            Move();
            CheckArrivedDestination();
        } else if (_externalForces == null) {
            SlowDown();
        }
        base.ProcessMovement();
    }

    public virtual Vector3 GetNextIdleDestination() {
        Vector3 randomLocation = transform.position + Random.onUnitSphere * 10f; // WIP: magic af number
        randomLocation.y = transform.position.y;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomLocation, out hit, 10f, NavMesh.AllAreas)) {
            return hit.position;
        }
        return transform.position;
    }

    public virtual void SetDestination(Vector3 target, float speed) {
        MoveSpeed = speed;
        _pathIndex = 0;
        _path.ClearCorners();
        Path = new Vector3[] { target };
        _agent.nextPosition = transform.position;
        bool success = _agent.CalculatePath(target, _path);
        if (success) {
            PathPending = true;
        } else {
            OnPathCalculated?.Invoke(NavMeshPathStatus.PathInvalid);
        }
        return;
    }

    protected void ProcessPathing() {
        if (!PathPending) {
            return;
        }
        if (_agent.pathPending) {
            return;
        }
        PathPending = false;
        OnPathSet();
    }

    private void OnPathSet() {
        PathPending = false;
        _agent.isStopped = true;
        _pathIndex = 0;
        _traveling = true;
        if (_path.corners.Length == 0) {
            OnPathCalculated?.Invoke(NavMeshPathStatus.PathComplete);
            return;
        }
        Path = _path.corners;
        NextPathCorner();
        _currentPathCorner = Path[_pathIndex];
        OnPathCalculated?.Invoke(_agent.pathStatus);
    }

    protected virtual void CheckArrivedDestination() {
        // stop moving when character has reached destination(path corner) character behaviour that we have reached destination
        Vector3 currentPosition = _npcBehaviour.transform.position;
        float distance = Vector3.Distance(currentPosition, _currentPathCorner);
        if (distance > _destinationArrivalDistance) {
            return;
        }
        // trigger event if you've arrived at destination
        if (!NextPathCorner()) {
            ArrivedDestination();
            return;
        }
        _currentPathCorner = Path[_pathIndex];
    }

    private void ArrivedDestination() {
        _traveling = false;
        float verticalVel = _movementVelocity.y;
        _movementVelocity = Vector3.zero;
        _movementVelocity.y = verticalVel;
        _path.ClearCorners();
        OnArrivedDestination?.Invoke();
    }

    public virtual bool IsPathObstructedToTarget(Vector3 target) {
        NavMeshHit hit;
        return NavMesh.Raycast(transform.position, target, out hit, NavMesh.AllAreas);
    }

    public virtual void ClearCurrentDestination() {
        _path.ClearCorners();
        _pathIndex = 0;
        _traveling = false;
    }
    
    // move the character at this speed in this direction
    protected virtual void Move() {
        if (_externalForces == null) {
            Vector3 moveDir = (_currentPathCorner - _npcBehaviour.transform.position).normalized;
            _movementVelocity.x = moveDir.x * MoveSpeed;
            _movementVelocity.z = moveDir.z * MoveSpeed;
        }
    }

    public virtual void Stop() {
        _traveling = false;
        _moveSpeed = 0f;
    }

    private void SlowDown() {
        float gravity = _movementVelocity.y;
        if (Mathf.Approximately(_movementVelocity.magnitude, 0f)) {
            _movementVelocity = Vector3.down * gravity;
            return;
        }
        _movementVelocity = Vector3.Lerp(_movementVelocity, Vector3.zero, 0.5f);
        _movementVelocity.y = gravity;
    }

    // sets the rotation of the character
    public virtual void SetRotation(Vector3 target) {
        Vector3 lookDirection = target - _npcBehaviour.Head.position;
        Vector3 lookDirectionBody = lookDirection;
        lookDirectionBody.y = 0;

        float step = _turnSpeed * Time.deltaTime;
        float radStep = Mathf.Deg2Rad * step;
        Vector3 newLook = Vector3.RotateTowards(transform.forward, lookDirectionBody, radStep, 0f);
        transform.rotation = Quaternion.LookRotation(newLook);
    }

    public virtual void SetRotation(Vector3 target, float value) {
        Vector3 lookDirection = target - _npcBehaviour.Head.position;
        Vector3 lookDirectionBody = lookDirection;
        lookDirectionBody.y = 0;

        Vector3 newLook = Vector3.Lerp(transform.forward, lookDirectionBody, value);
        transform.rotation = Quaternion.LookRotation(newLook);
    }
}
