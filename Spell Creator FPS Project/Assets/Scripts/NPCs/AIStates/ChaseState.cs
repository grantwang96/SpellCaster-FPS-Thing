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
        _moveController.OnPathCalculated += OnPathCalculated;
        _moveController.OnArrivedDestination += OnArriveDestination;
        if (_vision.CurrentTarget == null) {
            Debug.Log("Target was null!");
            _npcBehaviour.ChangeBrainState(_onTargetLostState);
            return;
        }
    }

    public override void Execute() {
        bool canSeeTarget = _vision.CanSeeTarget(_vision.CurrentTarget.GetBodyPosition());
        if (_npcBehaviour.Blueprint.CanAttack(_npcBehaviour, _vision.CurrentTarget)) {
            _npcBehaviour.ChangeBrainState(_onTargetReachedState);
        }
        _moveController.SetRotation(targetLastKnownPosition);
        if (canSeeTarget) {
            targetLastKnownPosition = _vision.CurrentTarget.transform.position;
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
        bool canSeeTarget = _vision.CanSeeTarget(_vision.CurrentTarget.GetBodyPosition());
        if (canSeeTarget) {
            _npcBehaviour.ChangeBrainState(_onTargetReachedState);
            return;
        }
        _npcBehaviour.ChangeBrainState(_onTargetLostState);
    }
}