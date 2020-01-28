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

public abstract class BrainState : MonoBehaviour {

    [SerializeField] protected BrainState[] _childrenStates; // sub states that should also be processing along with this parent state
    [SerializeField] protected NPCBehaviour _npcBehaviour;

    [SerializeField] protected AnimationData _enterAnimationData;
    [SerializeField] protected AnimationData _exitAnimationData;
    [SerializeField] protected NPCAnimController _animController;

    protected float _startTime;
    protected float _duration;

    protected List<BrainState> _validBrainStates = new List<BrainState>();

    protected virtual void Awake() {

    }

    public virtual void Enter(BrainState overrideBrainState = null, float duration = 0f) {
        _animController.OnAnimationStateUpdated += OnAnimationStateUpdated;
        foreach (BrainState brainState in _childrenStates) {
            brainState.Enter(overrideBrainState, duration);
        }
        _animController.UpdateAnimationData(_enterAnimationData);
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
        _animController.UpdateAnimationData(_exitAnimationData);
        _animController.OnAnimationStateUpdated -= OnAnimationStateUpdated;
    }

    public virtual bool CanTransition() {
        return true;
    }

    protected void GetValidBrainStateTransitions(BrainState[] brainStates) {
        _validBrainStates.Clear();
        for(int i = 0; i < brainStates.Length; i++) {
            if (brainStates[i].CanTransition()) {
                _validBrainStates.Add(brainStates[i]);
            }
        }
    }

    protected virtual void OnAnimationStateUpdated(AnimationState state) {

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
