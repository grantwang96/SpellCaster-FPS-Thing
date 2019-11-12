﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveState : BrainState {
    
    // for the normal move state, you probably should just set "idle" as the one brain state in array
    [SerializeField] protected BrainState[] _onTargetReachedStates;
    [SerializeField] protected BrainState _onFailedToReachTargetState;
    [SerializeField] private float _overrideCancelTime; // if moving there takes too long

    [SerializeField] protected NPCMoveController _moveController;
    
    private bool _facingTarget;
    private float _stateTime;

    public override void Enter(BrainState overrideBrainState = null, float duration = 0f) {
        base.Enter(overrideBrainState, duration);
        _stateTime = 0f;
        Vector3 targetDestination = GetDestination();
        _moveController.OnPathCalculated += OnPathCalculated;
        _moveController.SetDestination(targetDestination);
    }

    protected override void SetTriggerName() {
        _triggerName = GameplayValues.BrainStates.WalkStateId;
    }

    protected virtual Vector3 GetDestination() {
        return _moveController.GetNextIdleDestination();
    }

    public override void Execute() {
        _stateTime += Time.deltaTime;
        if(_stateTime > _overrideCancelTime && _overrideCancelTime > 0f) {
            _npcBehaviour.ChangeBrainState(_onFailedToReachTargetState);
            return;
        }
        if (_moveController.PathPending) { Debug.Log(_npcBehaviour.name + " path is pending..."); return; }
        SetRotation();
        base.Execute();
    }

    protected virtual void SetRotation() {
        Vector3 lookTarget = _moveController.CurrentPathCorner;
        _npcBehaviour.CharMove.SetRotation(lookTarget);
    }

    protected virtual void OnPathCalculated(NavMeshPathStatus status) {
        if(status == NavMeshPathStatus.PathInvalid) {
            Debug.Log("Could not calculate path to target!");
            _npcBehaviour.ChangeBrainState(_onTargetReachedStates[0]);
            return;
        }
        _moveController.OnArrivedDestination += OnArriveDestination;
    }

    protected virtual void OnArriveDestination() {
        _npcBehaviour.ChangeBrainState(_onTargetReachedStates[0]);
    }
    
    public override void Exit() {
        _moveController.OnPathCalculated -= OnPathCalculated;
        _moveController.OnArrivedDestination -= OnArriveDestination;
        _moveController.ClearCurrentDestination();
        base.Exit();
    }
}

