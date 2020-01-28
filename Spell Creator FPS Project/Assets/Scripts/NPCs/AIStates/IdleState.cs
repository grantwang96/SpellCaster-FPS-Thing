using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BrainState {

    [SerializeField] private BrainState _onIdleEnd;
    [SerializeField] private float _defaultIdleTime;

    [SerializeField] private NPCMoveController _moveController;

    public override void Enter(BrainState overrideBrainState = null, float duration = 0f) {
        base.Enter(overrideBrainState, duration);
        _startTime = Time.time;
        _duration = Mathf.Approximately(duration, 0f) ? _defaultIdleTime : duration;
        _npcBehaviour.CharMove.Stop();
    }

    public override void Execute() {
        base.Execute();
        // if duration is less than 0, wait for override brain state change
        if (_duration < 0f) {
            return;
        }
        // idle time is over, change to walk mode
        if (Time.time - _startTime > _duration) {
            _npcBehaviour.ChangeBrainState(_onIdleEnd);
        }
    }
}
