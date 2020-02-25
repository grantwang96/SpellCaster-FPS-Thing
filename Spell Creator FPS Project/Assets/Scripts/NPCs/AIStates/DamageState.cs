using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageState : BrainState
{
    [SerializeField] protected BrainState[] _targetKnownStates;
    [SerializeField] protected BrainState[] _targetUnknownStates;

    [SerializeField] private NPCVision _npcVision;

    protected override void OnAnimationStateUpdated(AnimationState state) {
        base.OnAnimationStateUpdated(state);
        if(state == AnimationState.CanTransition) {

        }
    }
}
