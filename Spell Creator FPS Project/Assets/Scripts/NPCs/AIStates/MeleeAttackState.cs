using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackState : AttackState {

    [SerializeField] private BrainState _onTargetInRangeState; // transition to this state if target is within this range
    [SerializeField] private BrainState _onTargetOutOfRangeState; // transition to this state if target is out of range
    [SerializeField] private List<HitboxInfo> _hitBoxInfos = new List<HitboxInfo>();

    private HitboxController _hitboxController;

    protected override void Awake() {
        base.Awake();
        _hitboxController = _npcBehaviour.GetComponent<HitboxController>();
    }

    public override void Enter(BrainState overrideBrainState = null, float duration = 0f, bool isChild = false) {
        _moveController.Stop();

        if(_hitboxController == null) {
            // fail here
            CustomLogger.Error(_npcBehaviour.name, $"Was not able to detect {nameof(HitboxController)} on character!");
            _npcBehaviour.ChangeBrainState(_onTargetOutOfRangeState);
            return;
        }

        for(int i = 0; i < _hitBoxInfos.Count; i++) {
            Hitbox hitbox;
            if(!_hitboxController.HitBoxes.TryGetValue(_hitBoxInfos[i].Id, out hitbox)) {
                CustomLogger.Error(_npcBehaviour.name, $"Could not find hitbox with ID \"{_hitBoxInfos[i].Id}\"");
                continue;
            }
            hitbox.Initialize(_hitBoxInfos[i]);
        }
        base.Enter(overrideBrainState, duration);
    }

    public override bool CanTransition() {
        if(_npcVision.CurrentTarget == null) {
            return false;
        }
        return TargetWithinRange(_npcVision.CurrentTarget.GetBodyPosition());
    }

    protected override void OnAnimationStateUpdated(AnimationState state) {
        base.OnAnimationStateUpdated(state);

        if(state == AnimationState.CanTransition) {
            TryTransitionToNextAttack();
            return;
        }
        if(state == AnimationState.Completed) {
            _npcBehaviour.ChangeBrainState(_onTargetOutOfRangeState);
        }
    }

    private void TryTransitionToNextAttack() {
        if (_onTargetInRangeState.CanTransition()) {
            _npcBehaviour.ChangeBrainState(_onTargetInRangeState);
        }
    }

    protected override void OnTakeDamage(DamageData data) {
        
    }
}
