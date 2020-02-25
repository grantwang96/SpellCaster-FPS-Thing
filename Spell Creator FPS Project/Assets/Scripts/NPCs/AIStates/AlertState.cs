using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertState : BrainState
{
    [SerializeField] private BrainState _onAlertEndState;
    [SerializeField] private NPCMoveController _npcMoveController;
    [SerializeField] private string _animationName;
    
    public override void Enter(BrainState overrideBrainState = null, float duration = 0, bool isChild = false) {
        _duration = duration;
        _startTime = Time.time;
        base.Enter(overrideBrainState, duration);
        _npcMoveController.Stop();
    }

    protected override void OnAnimationStateUpdated(AnimationState state) {
        base.OnAnimationStateUpdated(state);

        if(state == AnimationState.Completed) {
            _npcBehaviour.ChangeBrainState(_onAlertEndState);
        }
    }

    protected override void OnTakeDamage(DamageData data) {
        // do nothing when taking damage
    }
}
