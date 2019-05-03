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
    private Vector3 targetLastKnownPosition;

    private void Awake() {
        _vision = _npcBehaviour.GetComponent<IVision>();
    }

    public override void Enter(BrainState overrideBrainState = null) {
        targetLastKnownPosition = _vision.CurrentTarget.transform.position;
        base.Enter(overrideBrainState);
        if (_vision.CurrentTarget == null) {
            Debug.Log("Target was null!");
            _npcBehaviour.ChangeBrainState(_onTargetLostState);
            return;
        }
    }

    protected override Vector3 GetDestination() {
        return _vision.CurrentTarget.transform.position;
    }

    public override void Execute() {
        if (TryAttack()) { return; }
        SetRotation();
        SetDestination();
    }

    protected override void SetRotation() {
        _moveController.SetRotation(targetLastKnownPosition);
    }

    private void SetDestination() {
        bool canSeeTarget = _vision.CanSeeTarget(_vision.CurrentTarget.GetBodyPosition());
        if (canSeeTarget) {
            targetLastKnownPosition = _vision.CurrentTarget.transform.position;
            _moveController.SetDestination(targetLastKnownPosition);
        }
    }

    private bool TryAttack() {
        if (_npcBehaviour.Blueprint.CanAttack(_npcBehaviour, _vision.CurrentTarget)) {
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
    }

    protected override void OnArriveDestination() {
        bool canSeeTarget = _vision.CanSeeTarget(_vision.CurrentTarget.GetBodyPosition());
        if (canSeeTarget) {
            _npcBehaviour.ChangeBrainState(_onTargetReachedStates[Random.Range(0, _onTargetReachedStates.Length)]);
            return;
        }
        _npcBehaviour.ChangeBrainState(_onTargetLostState);
    }
}