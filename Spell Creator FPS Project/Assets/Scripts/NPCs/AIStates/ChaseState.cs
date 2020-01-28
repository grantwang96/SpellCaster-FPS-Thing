﻿using System.Collections;
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

    public override void Enter(BrainState overrideBrainState = null, float duration = 0f) {
        if (_vision.CurrentTarget == null) {
            _npcBehaviour.ChangeBrainState(_onTargetLostState);
            return;
        }
        base.Enter(overrideBrainState, duration);
        _moveSpeed = _moveController.MaxSpeed;
    }

    protected override Vector3 GetDestination() {
        _targetLastKnownPosition = _vision.CurrentTarget.transform.position;
        return _targetLastKnownPosition;
    }

    public override void Execute() {
        base.Execute();
        // if (TryAttack()) { return; }
        SetDestination();
    }

    protected override void SetRotation() {
        _moveController.SetRotation(_targetLastKnownPosition);
    }

    private void SetDestination() {
        bool targetInView = _vision.CheckVision(_vision.CurrentTarget);
        if (targetInView) {
            _targetLastKnownPosition = _vision.CurrentTarget.transform.position;
            _moveController.SetDestination(_targetLastKnownPosition, _moveSpeed);
        }
    }

    private bool TryAttack() {
        GetValidBrainStateTransitions(_onTargetReachedStates);
        if(_validBrainStates.Count == 0) {
            return false;
        }
        _moveController.ClearCurrentDestination();
        _npcBehaviour.ChangeBrainState(_validBrainStates[Random.Range(0, _validBrainStates.Count)]);
        return true;
    }

    protected override void OnPathCalculated(NavMeshPathStatus status) {
        if (status == NavMeshPathStatus.PathInvalid) {
            _npcBehaviour.ChangeBrainState(_onTargetLostState);
            return;
        }
        _moveController.OnArrivedDestination += OnArriveDestination;
    }

    protected override void OnArriveDestination() {
        if (TryAttack()) {
            return;
        }
        bool canSeeTarget = _vision.CanSeeTarget(_vision.CurrentTarget.GetBodyPosition());
        if (canSeeTarget) {
            _npcBehaviour.ChangeBrainState(_onTargetSeenState);
            return;
        }
        _npcBehaviour.ChangeBrainState(_onTargetLostState);
    }
}