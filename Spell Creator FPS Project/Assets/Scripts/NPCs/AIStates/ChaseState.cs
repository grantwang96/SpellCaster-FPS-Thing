using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// movement specifically during combat
public class ChaseState : MoveState {
    
    [SerializeField] private BrainState _onTargetSeenState;
    [SerializeField] private BrainState _onTargetOutOfSightState;
    [SerializeField] private BrainState _onTargetLostState;

    private Vector3 targetLastKnownPosition;

    public override void Enter(NPCBehaviour behaviour) {
        targetLastKnownPosition = behaviour.CurrentTarget.transform.position;
        _npcBehaviour.targetDestination = targetLastKnownPosition;
        _moveController.OnPathCalculated += OnPathCalculated;
        _moveController.OnArrivedDestination += OnArriveDestination;
        if (_npcBehaviour.CurrentTarget == null) {
            _npcBehaviour.ChangeBrainState(_onTargetLostState);
            return;
        }
    }

    public override void Execute() {
        bool canSeeTarget = _npcBehaviour.CanSeeTarget(_npcBehaviour.CurrentTarget.GetBodyPosition());
        if (_npcBehaviour.Blueprint.CanAttack(_npcBehaviour)) {
            _npcBehaviour.ChangeBrainState(_onTargetReachedState);
        }
        _npcBehaviour.CharMove.SetRotation(targetLastKnownPosition);
        if (canSeeTarget) {
            targetLastKnownPosition = _npcBehaviour.CurrentTarget.transform.position;
            _moveController.SetDestination(targetLastKnownPosition);
        }
    }

    protected override void OnPathCalculated(NavMeshPathStatus status) {
        if (status == NavMeshPathStatus.PathInvalid) {
            _npcBehaviour.ChangeBrainState(_onTargetLostState);
            return;
        }
    }

    protected override void OnArriveDestination() {
        bool canSeeTarget = _npcBehaviour.CanSeeTarget(_npcBehaviour.CurrentTarget.GetBodyPosition());
        if (canSeeTarget) {
            _npcBehaviour.ChangeBrainState(_onTargetReachedState);
            return;
        }
        _npcBehaviour.ChangeBrainState(_onTargetLostState);
    }
}