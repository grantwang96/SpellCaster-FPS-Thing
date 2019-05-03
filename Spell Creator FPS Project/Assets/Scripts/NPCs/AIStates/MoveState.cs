using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveState : BrainState {
    
    // for the normal move state, you probably should just set "idle" as the one brain state in array
    [SerializeField] protected BrainState[] _onTargetReachedStates;
    [SerializeField] private float _overrideCancelTime;

    [SerializeField] protected NPCMoveController _moveController;

    private bool _pathCalculated;
    private bool _facingTarget;

    public override void Enter(BrainState overrideBrainState = null) {
        base.Enter(overrideBrainState);
        Vector3 targetDestination = GetDestination();
        _moveController.OnPathCalculated += OnPathCalculated;
        _pathCalculated = false;
        // _npcBehaviour.Blueprint.OnMoveEnter(_npcBehaviour);
        if (!_moveController.SetDestination(targetDestination)) {
            _npcBehaviour.ChangeBrainState(_onTargetReachedStates[0]);
            return;
        }
    }

    protected virtual Vector3 GetDestination() {
        return _moveController.GetNextIdleDestination();
    }

    public override void Execute() {
        if (!_pathCalculated) { Debug.Log(_npcBehaviour.name + " path is pending..."); return; }
        SetRotation();
        base.Execute();
    }

    protected virtual void SetRotation() {
        Vector3 lookTarget = _moveController.CurrentPathCorner;
        _npcBehaviour.CharMove.SetRotation(lookTarget);
    }

    protected virtual void OnPathCalculated(NavMeshPathStatus status) {
        if(status == NavMeshPathStatus.PathInvalid) {
            _npcBehaviour.ChangeBrainState(_onTargetReachedStates[0]);
            return;
        }
        _moveController.OnArrivedDestination += OnArriveDestination;
        _pathCalculated = true;
    }

    protected virtual void OnArriveDestination() {
        _npcBehaviour.ChangeBrainState(_onTargetReachedStates[0]);
    }
    
    public override void Exit() {
        _moveController.OnPathCalculated -= OnPathCalculated;
        _moveController.OnArrivedDestination -= OnArriveDestination;
        base.Exit();
    }
}

