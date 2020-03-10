using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimationStateBroadcaster : MonoBehaviour
{
    public event Action<AnimationState> OnAnimationStateUpdated;

    private void OnStateUpdated(AnimationState state) {
        OnAnimationStateUpdated?.Invoke(state);
    }
}
