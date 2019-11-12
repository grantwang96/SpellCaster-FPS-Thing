using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// movement specifically during combat
public class ChaseState : MoveState {
    
    [SerializeField] private BrainState _onTargetSeenState;
    [SerializeField] private BrainState _onTargetOutOfSightState;
    [SerializeField] private BrainState _onTargetLostState;

    private IVision _vision;
    [SerializeField] private Vector3 _targetLastKnownPosition;

    private void Start() {
        _vision = _npcBehaviour.GetComponent<IVision>();
    }

    protected override void SetTriggerName() {
        _triggerName = GameplayValues.BrainStates.ChaseStateId;
    }

    public override void Enter(BrainState overrideBrainState = null, float duration = 0f) {
        Debug.Log("Chase State entered!");
        if (_vision.CurrentTarget == null) {
            _npcBehaviour.ChangeBrainState(_onTargetLostState);
            return;
        }
        base.Enter(overrideBrainState, duration);
    }

    protected override Vector3 GetDestination() {
        _targetLastKnownPosition = _vision.CurrentTarget.transform.position;
        return _targetLastKnownPosition;
    }

    public override void Execute() {
        base.Execute();
        if (TryAttack()) { return; }
        SetDestination();
    }

    protected override void SetRotation() {
        _moveController.SetRotation(_targetLastKnownPosition);
    }

    private void SetDestination() {
        bool targetInView = _vision.CheckVision(_vision.CurrentTarget);
        if (targetInView) {
            if(_onTargetSeenState != null) {
                _npcBehaviour.ChangeBrainState(_onTargetSeenState);
                return;
            }
            _targetLastKnownPosition = _vision.CurrentTarget.transform.position;
            _moveController.SetDestination(_targetLastKnownPosition);
        }
    }

    private bool TryAttack() {
        if (_npcBehaviour.Blueprint.CanAttack(_npcBehaviour, _vision.CurrentTarget)) {
            _moveController.ClearCurrentDestination();
            _npcBehaviour.ChangeBrainState(_onTargetReachedStates[Random.Range(0, _onTargetReachedStates.Length)]);
            return true;
        }
        return false;
    }

    protected override void OnPathCalculated(NavMeshPathStatus status) {
        if (status == NavMeshPathStatus.PathInvalid) {
            _npcBehaviour.ChangeBrainState(_onTargetLostState);
            return;
        }
        _moveController.OnArrivedDestination += OnArriveDestination;
    }

    protected override void OnArriveDestination() {
        bool canSeeTarget = _vision.CanSeeTarget(_vision.CurrentTarget.GetBodyPosition());
        if (canSeeTarget) {
            _npcBehaviour.ChangeBrainState(_onTargetSeenState);
            return;
        }
        _npcBehaviour.ChangeBrainState(_onTargetLostState);
    }
}