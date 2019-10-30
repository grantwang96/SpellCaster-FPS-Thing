using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BrainStateTransitionId {
    Idle,
    Move,
    Alert,
    Chase,
    TakeCover,
    MeleeAttack,
    RangeAttack
}

public abstract class BrainState : MonoBehaviour{

    [SerializeField] protected string _triggerName;
    public string TriggerName { get { return _triggerName; } }
    [SerializeField] protected BrainState[] _childrenStates; // sub actions that should also be processing along with this parent state
    [SerializeField] protected NPCBehaviour _npcBehaviour;

    protected float _startTime;
    protected float _duration;

    public virtual void Enter(BrainState overrideBrainState = null, float duration = 0f) {
        foreach(BrainState brainState in _childrenStates) {
            brainState.Enter(overrideBrainState, duration);
        }
    }
    public virtual void Execute() {
        foreach (BrainState brainState in _childrenStates) {
            brainState.Execute();
        }
    }
    public virtual void Exit() {
        // Apply any final changes/calculations before switching to new state
        foreach (BrainState brainState in _childrenStates) {
            brainState.Exit();
        }
    }
}

public class TransitionState : BrainState {

    public TransitionState(float time) : base() {

    }
}

[System.Serializable]
public class BrainStateTransition {
    [SerializeField] private BrainStateTransitionId _transitionId;
    public BrainStateTransitionId TransitionId => _transitionId;
    [SerializeField] private BrainState _brainState;
    public BrainState BrainState => _brainState;
}
