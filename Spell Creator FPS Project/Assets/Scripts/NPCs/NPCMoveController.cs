using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMoveController : CharacterMoveController {

    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected float _turnSpeed;
    [SerializeField] protected float _destinationArrivalDistance; // distance NPC is considered "arrived" at destination

    protected NPCBehaviour _npcBehaviour; // gain read access from character's brain
    protected NavMeshAgent _agent;

    [SerializeField] protected Vector3 _currentPathCorner; // what the NPC is moving towards(usually path corner)
    public Vector3 CurrentPathCorner { get { return _currentPathCorner; } }
    protected Vector3 _currentLookTarget; // what the NPC is looking towards
    public Vector3 CurrentLookTarget { get { return _currentLookTarget; } }

    protected bool _traveling;
    private bool _pathPending;

    public delegate void PathCalculatedDelegate(NavMeshPathStatus pathStatus);
    public event PathCalculatedDelegate OnPathCalculated; // called when path is finished calculating
    public delegate void ArrivedDestinationDelegate();
    public event ArrivedDestinationDelegate OnArrivedDestination;

    protected NavMeshPath _path;
    public Vector3[] Path { get { return _path.corners; } }
    [SerializeField] protected int _pathIndex;

    public bool NextPathCorner() {
        if (_pathIndex + 1 >= Path.Length) { return false; }
        _pathIndex++;
        return true;
    }

    protected override void Awake() {
        base.Awake();
        _npcBehaviour = GetComponent<NPCBehaviour>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.updatePosition = false;
        _agent.updateRotation = false;
    }

    protected override void Start() {
        base.Start();
        _baseSpeed = _npcBehaviour.Blueprint.WalkSpeed;
        _maxSpeed = _npcBehaviour.Blueprint.RunSpeed;
        _destinationArrivalDistance = _npcBehaviour.Blueprint.AttackRange;
    }

    protected override void Update() {
        ProcessMovement();
    }

    // move character towards current destination if traveling
    protected virtual void ProcessMovement() {
        if (_traveling) {
            Move();
            CheckArrivedDestination();
        } else {
            SlowDown();
        }
    }

    public virtual Vector3 GetNextDestination() {
        Vector3 randomLocation = transform.position + Random.onUnitSphere * 10f; // WIP: magic af number
        randomLocation.y = transform.position.y;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomLocation, out hit, 10f, NavMesh.AllAreas)) {
            return hit.position;
        }
        return transform.position;
    }

    public virtual bool SetDestination(Vector3 target) {
        if (_agent.pathPending) { StopCoroutine(CalculatePath()); }
        _path = new NavMeshPath();
        _agent.nextPosition = transform.position;
        bool success = _agent.SetDestination(target);
        StartCoroutine(CalculatePath());
        return success;
    }

    private IEnumerator CalculatePath() {
        _pathPending = true;
        while (_agent.pathPending) { yield return null; }
        _pathPending = false;
        _agent.isStopped = true;
        _path = _agent.path;
        _pathIndex = 0;
        NextPathCorner();
        _currentPathCorner = Path[_pathIndex];
        _traveling = true;
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
            _traveling = false;
            _movementVelocity = Vector3.zero;
            _path.ClearCorners();
            OnArrivedDestination?.Invoke();
            return;
        }
        _currentPathCorner = Path[_pathIndex];
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
            _movementVelocity.x = moveDir.x * _moveSpeed;
            _movementVelocity.z = moveDir.z * _moveSpeed;
        }
    }

    public virtual void Stop() {
        _traveling = false;
    }

    private void SlowDown() {
        // BUG: NEED TO ACCOUNT FOR GRAVITY
        if(Mathf.Approximately(_movementVelocity.magnitude, 0f)) {
            _movementVelocity = Vector3.zero;
            return;
        }
        _movementVelocity = Vector3.Lerp(_movementVelocity, Vector3.zero, 0.5f);
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
}
