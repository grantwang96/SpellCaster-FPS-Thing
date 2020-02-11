using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleAnimationState : BrainState
{
    [SerializeField] private BrainState _nextState;

    protected override void OnAnimationStateUpdated(AnimationState state) {
        base.OnAnimationStateUpdated(state);

        if (state == AnimationState.CanTransition) {
            _npcBehaviour.ChangeBrainState(_nextState);
        }
    }
}
