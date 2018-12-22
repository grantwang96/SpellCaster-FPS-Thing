using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NPCMoveController))]
public class NPCBehaviour : CharacterBehaviour, IVision {

    [SerializeField] protected int _health;
    public int Health { get { return _health; } }

    [SerializeField] protected NPCMoveController _charMove;
    public NPCMoveController CharMove { get { return _charMove; } }
    [SerializeField] protected NPCBlueprint blueprint; // blueprint to derive data from
    public NPCBlueprint Blueprint { get { return blueprint; } }
    protected BrainState currentBrainState; // current state of AI State Machine

    // where pathfinding is handled (do not allow agent to move character)
    [SerializeField] protected NavMeshAgent _agent;
    public NavMeshAgent Agent { get { return _agent; } }
    [SerializeField] protected Animator _animator;
    public Animator Animator { get { return _animator; } }
    
    // characters this NPC is aware of
    [SerializeField] protected List<CharacterBehaviour> knownCharacters = new List<CharacterBehaviour>();
    public List<CharacterBehaviour> KnownCharacters { get { return knownCharacters; } }
    // if the NPC is tracking another Character, this is the target they are tracking
    protected CharacterBehaviour currentTarget;
    public CharacterBehaviour CurrentTarget { get { return currentTarget; } }

    protected NavMeshPath _path;
    [SerializeField] protected int pathIndex;
    public Vector3[] Path { get { return _path.corners; } }
    public Vector3 currentDestination {
        get {
            if(_path == null || _path.corners.Length == 0) {
                Debug.LogWarning(name + ": currentDestination called, but path is empty!");
                return transform.position + transform.forward;
            }
            return _path.corners[pathIndex];
        }

    }
    public bool NextPathCorner() {
        if (pathIndex + 1 >= Path.Length) { return false; }
        pathIndex++;
        return true;
    }

    // HACK: REMOVE/MODIFY LATER
    public Vector3 targetDestination;

    protected override void Awake() {
        base.Awake();
        _charMove = GetComponent<NPCMoveController>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.updatePosition = false;
        _agent.updateRotation = false;
        _animator = BodyTransform.GetComponent<Animator>();
    }

    protected virtual void Start() {
        if(blueprint == null) {
            Debug.LogError(name + " doesn't have a blueprint!");
            return;
        }
        _health = blueprint.TotalHealth;
        BaseSpeed = blueprint.WalkSpeed;
        MaxSpeed = blueprint.RunSpeed;
    }

    protected virtual void Update() {
        if(currentBrainState != null) { currentBrainState.Execute(); }
    }

    public override float GetMoveMagnitude() {
        Vector3 vel = _charMove.CharacterController.velocity;
        vel.y = 0f;
        return vel.magnitude;
    }

    /// <summary>
    /// Changes the current state in the AI State Machine
    /// </summary>
    /// <param name="brainState"></param>
    public virtual void ChangeBrainState(BrainState brainState) {
        // perform any exit operations from the previous state
        if (currentBrainState != null) { currentBrainState.Exit(); }

        // save the new brain state and enter
        currentBrainState = brainState;
        currentBrainState.Enter(this);

        string stateName = currentBrainState.GetStateName();
        InvokeChangeAnimationState(stateName);
    }

    /// <summary>
    /// If a character has been seen, this NPC should react accordingly
    /// </summary>
    /// <param name="behaviour"></param>
    protected virtual void ReactToCharacter(CharacterBehaviour behaviour) {
        if(behaviour.Friendly != Friendly) {
            currentTarget = behaviour;
            ChangeBrainState(new ChaseState());
        }
    }
    
    public virtual bool CalculatePath(Vector3 destination) {
        if (_agent.pathPending) { StopCoroutine(PathPending()); }
        _path = new NavMeshPath();
        pathIndex = 0;
        Agent.nextPosition = transform.position;
        bool success = Agent.SetDestination(destination);
        StartCoroutine(PathPending());
        return success;
    }

    private IEnumerator PathPending() {
        while (_agent.pathPending) { yield return null; }
        Agent.isStopped = true;
        _path = _agent.path;
    }

    public virtual bool CheckPathSegment(Vector3 start, Vector3 end) {
        NavMeshHit hit;
        if (NavMesh.Raycast(start, end, out hit, NavMesh.AllAreas)) {
            return false;
        }
        return true;
    }

    public virtual Vector3 GetNextDestination() {
        Vector3 randomLocation = transform.position + Random.onUnitSphere * 10f;
        randomLocation.y = transform.position.y;

        NavMeshHit hit;
        if(NavMesh.SamplePosition(randomLocation, out hit, 10f, NavMesh.AllAreas)) {
            return hit.position;
        }
        return transform.position;
    }

    public virtual void CheckVision() {
        // TODO: IMPLEMENT THIS FUNCTION
        for(int i = 0; i < KnownCharacters.Count; i++) {
            CharacterBehaviour knownCharacter = KnownCharacters[i];
            float distance = Vector3.Distance(knownCharacter.transform.position, transform.position);
            float angle = Vector3.Angle(transform.forward, knownCharacter.transform.position - transform.position);
            if(distance <= Blueprint.VisionRange && angle <= Blueprint.VisionAngle) {
                Vector3 dir = knownCharacter.GetBodyPosition() - Head.position;
                RaycastHit hit;
                Debug.DrawRay(Head.position, dir, Color.red);
                if (Physics.Raycast(Head.position, dir, out hit, blueprint.VisionRange, blueprint.VisionMask)) {
                    CharacterBehaviour otherCB = hit.transform.GetComponent<CharacterBehaviour>();
                    if (otherCB != null && knownCharacters.Contains(otherCB)) {
                        ReactToCharacter(otherCB);
                    }
                }
            }
        }
    }

    public virtual bool CanSeeTarget(Vector3 target) {
        float distance = Vector3.Distance(target, Head.position);
        Vector3 targetDir = target - Head.position;
        Vector3 headForward = Head.forward;
        headForward.y = 0f;
        if (distance < Blueprint.VisionRange) {
            RaycastHit hit;
            if(Physics.Raycast(Head.position, targetDir, out hit, distance, Blueprint.VisionMask)) {
                if(hit.transform == CurrentTarget.transform) { return true; }
            }
        }
        return false;
    }

    public virtual void ClearCurrentTarget() {
        currentTarget = null;
    }

    public virtual void RegisterToKnownCharacters(CharacterBehaviour characterBehaviour) {
        if (!knownCharacters.Contains(characterBehaviour)) {
            knownCharacters.Add(characterBehaviour);
        }
    }

    public virtual void DeregisterFromKnownCharacters(CharacterBehaviour characterBehaviour) {
        if (knownCharacters.Contains(characterBehaviour)) {
            knownCharacters.Remove(characterBehaviour);
        }
    }
}

/// <summary>
/// Interface that allows NPCs to check for vision
/// </summary>
public interface IVision {
    // WIP: these should return values
    
    void CheckVision(); // checks general vision and returns first custom object it sees
    bool CanSeeTarget(Vector3 target); // checks to see if this target is viewable(if it has one)
    void RegisterToKnownCharacters(CharacterBehaviour characterBehaviour);
    void DeregisterFromKnownCharacters(CharacterBehaviour characterBehaviour);
}
