using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BrainStateTransitionId {
    Idle,
    Move,
    Search,
    Alert,
    Chase,
    TakeCover,
    MeleeAttack,
    RangeAttack,
    TakeDamage,
    Hitstun,
    Death
}

public abstract class BrainState : MonoBehaviour {

    [SerializeField] protected BrainState[] _childrenStates; // sub states that should also be processing along with this parent state
    [SerializeField] protected NPCBehaviour _npcBehaviour;
    [SerializeField] protected NPCVision _npcVision;

    [SerializeField] protected AnimationData _enterAnimationData;
    [SerializeField] protected AnimationData _exitAnimationData;
    [SerializeField] protected NPCAnimController _animController;

    protected float _startTime;
    protected float _duration;

    protected List<BrainState> _validBrainStates = new List<BrainState>();
    protected BrainState _overrideNextState;

    protected virtual void Awake() {

    }

    public virtual void Enter(BrainState overrideNextState = null, float duration = 0f, bool isChild = false) {
        _overrideNextState = overrideNextState;
        _animController.OnAnimationStateUpdated += OnAnimationStateUpdated;
        foreach (BrainState brainState in _childrenStates) {
            brainState.Enter(null, duration, true);
        }
        _animController.UpdateAnimationData(_enterAnimationData);

        if (!isChild) {
            _npcBehaviour.Damageable.OnDamaged += OnTakeDamage;
            _npcBehaviour.Damageable.OnStun += OnEnterHitStun;
            _npcBehaviour.Damageable.OnKnockback += OnEnterHitStun;
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
        _animController.UpdateAnimationData(_exitAnimationData);
        _animController.OnAnimationStateUpdated -= OnAnimationStateUpdated;

        _npcBehaviour.Damageable.OnDamaged -= OnTakeDamage;
        _npcBehaviour.Damageable.OnStun -= OnEnterHitStun;
        _npcBehaviour.Damageable.OnKnockback -= OnEnterHitStun;

        _overrideNextState = null;
    }

    public virtual bool CanTransition() {
        return true;
    }

    protected void GetValidBrainStateTransitions(BrainState[] brainStates) {
        _validBrainStates.Clear();
        if (_overrideNextState != null) {
            _validBrainStates.Add(_overrideNextState);
            return;
        }
        for (int i = 0; i < brainStates.Length; i++) {
            if (brainStates[i].CanTransition()) {
                _validBrainStates.Add(brainStates[i]);
            }
        }
    }

    protected virtual void OnAnimationStateUpdated(AnimationState state) {

    }

    protected virtual void OnTakeDamage(DamageData data) {
        if (_npcVision.CurrentTarget != null) {
            return;
        }
        CharacterBehaviour character = data.Attacker.Root.GetComponent<CharacterBehaviour>();
        if (character == null) {
            return;
        }
        if (_npcVision.CheckVisionRadial(character)) {
            _npcVision.SetCurrentTarget(character);
        }
        _npcBehaviour.ChangeBrainState(BrainStateTransitionId.Alert);
    }

    protected virtual void OnEnterHitStun(Vector3 direction, float power) {
        _npcBehaviour.ChangeBrainState(BrainStateTransitionId.TakeDamage);
    }
}

[System.Serializable]
public class BrainStateTransition {
    [SerializeField] private BrainStateTransitionId _transitionId;
    public BrainStateTransitionId TransitionId => _transitionId;
    [SerializeField] private BrainState _brainState;
    public BrainState BrainState => _brainState;
}
