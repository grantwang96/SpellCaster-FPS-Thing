using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageState : SingleAnimationState
{
    [SerializeField] protected BrainState[] _targetKnownStates;
    [SerializeField] protected BrainState[] _targetUnknownStates;

    protected override void OnCanTransition() {

    }

    protected override void OnComplete() {
        if (_npcVision.CheckVisionRadial(_npcVision.CurrentTarget)) {

        }
    }
}
