using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCBehaviour : CharacterBehaviour, IVision, IDamageable {

    [SerializeField] protected int _health;
    public int Health;

    [SerializeField] protected CharacterMoveController _charMove;
    public CharacterMoveController CharMove { get { return _charMove; } }
    [SerializeField] protected NPCBlueprint blueprint; // blueprint to derive data from
    public NPCBlueprint Blueprint { get { return blueprint; } }
    protected BrainState currentBrainState; // current state of AI State Machine

    // where pathfinding is handled (do not allow agent to move character)
    [SerializeField] protected NavMeshAgent _agent;
    public NavMeshAgent Agent { get { return _agent; } }

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
        _charMove = GetComponent<CharacterMoveController>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.updatePosition = false;
        _agent.updateRotation = false;
    }

    protected virtual void Start() {
        if(blueprint == null) {
            Debug.LogError(name + " doesn't have a blueprint!");
            return;
        }
        _health = blueprint.TotalHealth;
    }

    protected virtual void Update() {
        if(currentBrainState != null) { currentBrainState.Execute(); }
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
    }

    /// <summary>
    /// If a character has been seen, this NPC should react accordingly
    /// </summary>
    /// <param name="behaviour"></param>
    protected virtual void ReactToCharacter(CharacterBehaviour behaviour) {

    }

    // TODO: GET RID OF THIS HACK IN THE FUTURE
    public void HACKSetDestination(Vector3 destination) {
        targetDestination = destination;
        ChangeBrainState(new MoveState(Blueprint.WalkSpeed));
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

    public virtual void TakeDamage(int damage) {
        Debug.Log(name + " received " + damage + " damage!");
    }

    public virtual void Die() {
        Debug.Log(name + " has died!");
    }

    public virtual void CheckVision() {
        // TODO: IMPLEMENT THIS FUNCTION
    }

    public virtual bool CanSeeTarget(Transform target) {
        float distance = Vector3.Distance(target.position, Head.position);
        Vector3 targetDir = target.position - Head.position;
        Vector3 targetDirZero = targetDir;
        targetDirZero.y = 0f;
        Vector3 headForward = Head.forward;
        headForward.y = 0f;
        float angle = Vector3.Angle(targetDirZero, headForward);
        if (distance < Blueprint.VisionRange && angle < Blueprint.VisionAngle) {
            RaycastHit hit;
            if(Physics.Raycast(Head.position, targetDir, out hit, distance, Blueprint.VisionMask)) {
                if(hit.transform == target.root) { return true; }
            }
        }
        return false;
    }
    
    public virtual bool DetectThreat() {
        // TODO: IMPLEMENT THIS FUNCTION
        return false;
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
    bool CanSeeTarget(Transform target); // checks to see if this target is viewable(if it has one)
    bool DetectThreat(); // checks for potential threat. Probably saves that info
    void RegisterToKnownCharacters(CharacterBehaviour characterBehaviour);
    void DeregisterFromKnownCharacters(CharacterBehaviour characterBehaviour);
}
