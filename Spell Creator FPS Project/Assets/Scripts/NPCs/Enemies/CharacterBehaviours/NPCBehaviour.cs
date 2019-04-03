using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NPCMoveController))]
public class NPCBehaviour : CharacterBehaviour {
    
    [SerializeField] protected NPCMoveController _charMove;
    public NPCMoveController CharMove { get { return _charMove; } }
    [SerializeField] protected Damageable _damageable;
    public Damageable Damageable { get; }
    [SerializeField] protected NPCBlueprint _blueprint; // blueprint to derive data from
    public NPCBlueprint Blueprint { get { return _blueprint; } }

    [SerializeField] protected BrainState _currentBrainState; // current state of AI State Machine
    [SerializeField] protected BrainState _startingState; // how the NPC should behave at start (usually idle)
    public delegate void BrainStateChangeDelegate(string newStateName);
    public event BrainStateChangeDelegate OnBrainStateChanged;
    
    // characters this NPC is aware of
    [SerializeField] protected List<CharacterBehaviour> knownCharacters = new List<CharacterBehaviour>();
    public List<CharacterBehaviour> KnownCharacters { get { return knownCharacters; } }
    [SerializeField] protected List<CharacterBehaviour> enemyCharacters = new List<CharacterBehaviour>();
    public List<CharacterBehaviour> EnemyCharacters { get { return enemyCharacters; } }
    // if the NPC is tracking another Character, this is the target they are tracking
    protected CharacterBehaviour _currentTarget;
    public CharacterBehaviour CurrentTarget { get { return _currentTarget; } }
    
    // HACK: REMOVE/MODIFY LATER
    public Vector3 targetDestination;

    protected override void Awake() {
        base.Awake();
        _charMove = GetComponent<NPCMoveController>();
        _damageable = GetComponent<Damageable>();
    }

    protected virtual void Start() {
        if(_blueprint == null) {
            Debug.LogError(name + " doesn't have a blueprint!");
            return;
        }
        BaseSpeed = _blueprint.WalkSpeed;
        MaxSpeed = _blueprint.RunSpeed;
        ChangeBrainState(_startingState);
    }

    protected virtual void Update() {
        if(_currentBrainState != null) { _currentBrainState.Execute(); }
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
        if (_currentBrainState != null) { _currentBrainState.Exit(); }

        // save the new brain state and enter
        _currentBrainState = brainState;
        _currentBrainState.Enter(this);

        string stateName = _currentBrainState.TriggerName;
        InvokeChangeAnimationState(stateName);
    }

    // funtion to raycast out to known characters
    public virtual bool CheckVision() {
        // TODO: IMPLEMENT THIS FUNCTION
        for(int i = 0; i < KnownCharacters.Count; i++) {
            CharacterBehaviour knownCharacter = KnownCharacters[i];
            float distance = Vector3.Distance(knownCharacter.transform.position, transform.position);
            float angle = Vector3.Angle(transform.forward, knownCharacter.transform.position - transform.position);
            if(distance <= Blueprint.VisionRange && angle <= Blueprint.VisionAngle) {
                Vector3 dir = knownCharacter.Head.position - Head.position;
                RaycastHit hit;
                Debug.DrawRay(Head.position, dir, Color.red);
                if (Physics.Raycast(Head.position, dir, out hit, _blueprint.VisionRange, _blueprint.VisionMask)) {
                    Debug.Log(hit.transform);
                    CharacterBehaviour otherCB = hit.transform.GetComponent<CharacterBehaviour>();
                    if (otherCB != null && knownCharacters.Contains(otherCB) && enemyCharacters.Contains(otherCB)) {
                        _currentTarget = otherCB;
                        return true;
                    }
                }
            }
        }
        return false;
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

    public virtual bool IsPathObstructedToTarget(Vector3 target) {
        NavMeshHit hit;
        return NavMesh.Raycast(transform.position, target, out hit, NavMesh.AllAreas);
    }

    public virtual void ClearCurrentTarget() {
        _currentTarget = null;
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

    protected virtual void OnDeath(bool isDead) {

    }
}

public interface INPCInitializable {
    void Initialize(NPCBlueprint blueprint);
}