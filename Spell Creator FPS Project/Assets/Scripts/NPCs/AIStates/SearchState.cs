using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchState : SingleAnimationState
{
    private bool _hasCurrentTarget;

    public override void Enter(BrainState overrideNextState = null, float duration = 0, bool isChild = false) {
        base.Enter(overrideNextState, duration, isChild);
        _hasCurrentTarget = _npcVision.CurrentTarget != null;
    }

    public override void Execute() {
        base.Execute();
        Search();
    }

    protected void Search() {
        bool foundTarget = false;
        if (_hasCurrentTarget) {
            foundTarget = _npcVision.CheckVision(_npcVision.CurrentTarget);
        }
        CharacterBehaviour target = _npcVision.CheckVision();
        if (target) {
            foundTarget = true;
            _npcVision.SetCurrentTarget(target);
        }
        if (foundTarget) {
            _npcBehaviour.ChangeBrainState(BrainStateTransitionId.Alert);
        }
    }

    protected override void OnCanTransition() {
        
    }
}
