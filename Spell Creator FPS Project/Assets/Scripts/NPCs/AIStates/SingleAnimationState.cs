using UnityEngine;

public class SingleAnimationState : BrainState
{
    [SerializeField] private BrainState _nextState;

    protected override void OnAnimationStateUpdated(AnimationState state) {
        base.OnAnimationStateUpdated(state);

        if (state == AnimationState.CanTransition) {
            OnCanTransition();
        }
        if(state == AnimationState.Completed) {
            OnComplete();
        }
    }

    protected virtual void OnCanTransition() {
        if (_overrideNextState != null) {
            _npcBehaviour.ChangeBrainState(_overrideNextState);
            return;
        }
        _npcBehaviour.ChangeBrainState(_nextState);
    }

    protected virtual void OnComplete() {
        if (_overrideNextState != null) {
            _npcBehaviour.ChangeBrainState(_overrideNextState);
            return;
        }
        _npcBehaviour.ChangeBrainState(_nextState);
    }
}
