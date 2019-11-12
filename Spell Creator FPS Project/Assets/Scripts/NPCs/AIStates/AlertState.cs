using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertState : BrainState
{
    [SerializeField] private BrainState _onAlertEndState;
    [SerializeField] private NPCAnimController _animController;
    [SerializeField] private NPCMoveController _npcMoveController;
    [SerializeField] private string _animationName;

    protected override void SetTriggerName() {
        _triggerName = GameplayValues.BrainStates.AlertStateId;
    }

    public override void Enter(BrainState overrideBrainState = null, float duration = 0) {
        _duration = duration;
        _startTime = Time.time;
        base.Enter(overrideBrainState, duration);
        _animController.PlayAnimation(_animationName);
        _npcMoveController.Stop();
    }

    public override void Execute() {
        base.Execute();
        // if duration is less than 0, wait for override brain state change
        if (_duration < 0f) {
            return;
        }
        float animDuration = _animController.GetCurrentAnimationDuration();
        float currentDuration = Time.time - _startTime;
        if(animDuration >= 1f && currentDuration >= _duration) {
            Debug.Log("Leave alert state");
            _npcBehaviour.ChangeBrainState(_onAlertEndState);
        }
    }
}
