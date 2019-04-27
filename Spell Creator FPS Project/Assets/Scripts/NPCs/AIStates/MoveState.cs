﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveState : BrainState {

    [SerializeField] protected BrainState _onTargetReachedState;
    [SerializeField] private float _overrideCancelTime;

    [SerializeField] protected NPCMoveController _moveController;

    private bool _pathCalculated;
    private bool _facingTarget;

    public override void Enter(BrainState overrideBrainState = null) {
        base.Enter(overrideBrainState);
        Vector3 targetDestination = _moveController.GetNextIdleDestination();
        _moveController.OnPathCalculated += OnPathCalculated;
        _pathCalculated = false;
        if (!_moveController.SetDestination(targetDestination)) {
            _npcBehaviour.ChangeBrainState(_onTargetReachedState);
            return;
        }
    }

    public override void Execute() {
        if (!_pathCalculated) { Debug.Log(_npcBehaviour.name + " path is pending..."); return; }
        Vector3 lookTarget = _moveController.CurrentPathCorner;
        _npcBehaviour.CharMove.SetRotation(lookTarget);
        base.Execute();
    }

    protected virtual void OnPathCalculated(NavMeshPathStatus status) {
        if(status == NavMeshPathStatus.PathInvalid) {
            _npcBehaviour.ChangeBrainState(_onTargetReachedState);
            return;
        }
        _moveController.OnArrivedDestination += OnArriveDestination;
        _pathCalculated = true;
    }

    protected virtual void OnArriveDestination() {
        _npcBehaviour.ChangeBrainState(_onTargetReachedState);
    }
    
    public override void Exit() {
        _moveController.OnPathCalculated -= OnPathCalculated;
        _moveController.OnArrivedDestination -= OnArriveDestination;
        base.Exit();
    }
}

