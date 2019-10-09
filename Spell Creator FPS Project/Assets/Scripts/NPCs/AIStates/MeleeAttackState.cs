using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackState : AttackState {

    [SerializeField] private BrainState _onTargetInRangeState; // transition to this state if target is within this range
    [SerializeField] private BrainState _onTargetOutOfRangeState; // transition to this state if target is out of range

    [SerializeField] private Collider[] _hitBoxes; // hit boxes to activate and deactivate on attacks
    [SerializeField] private float _timeActivateHitBoxes; // when to activate hitboxes
    [SerializeField] private float _timeDeactivateHitBoxes; // when to deactivate hitboxes
    [SerializeField] private int _attackComboIndex = -1;

    private bool _hitBoxesActive;
    private bool HitBoxesActive {
        get { return _hitBoxesActive; }
        set {
            _hitBoxesActive = value;
            SetHitBoxesActive(_hitBoxesActive);
        }
    }

    private void Awake() {
        if (_attackComboIndex > -1) {
            _attackName = $"{GameplayValues.Combat.NPCNormalAttackTriggerId}{_attackComboIndex}";
        }
    }

    public override void Enter(BrainState overrideBrainState = null) {
        base.Enter(overrideBrainState);
        if (_attackComboIndex > -1) {
            _animController.SetIntParameter(GameplayValues.Combat.NPCAttackComboIndexId, _attackComboIndex);
            _animController.SetTrigger(GameplayValues.Combat.NPCNormalAttackTriggerId);
            
        } else {
            _animController.PlayAnimation(_attackName);
        }
        HitBoxesActive = false;
        _moveController.Stop();
    }

    public override void Execute() {
        base.Execute();
        // wait for animation to start
        if (!_animController.IsStateByName(_attackName)) {
            return;
        }
        float currentTime = _animController.GetCurrentAnimationTime();
        if(currentTime >= _timeActivateHitBoxes && currentTime < _timeDeactivateHitBoxes && !HitBoxesActive) {
            HitBoxesActive = true;
        } else if (currentTime > _timeDeactivateHitBoxes && HitBoxesActive) {
            HitBoxesActive = false;
            if (_npcBehaviour.Blueprint.CanAttack(_npcBehaviour, _npcVision.CurrentTarget)) {
                _npcBehaviour.ChangeBrainState(_onTargetInRangeState);
                return;
            }
        }
        if (currentTime >= 1f) {
            _npcBehaviour.ChangeBrainState(_onTargetOutOfRangeState);
        }
    }

    public override void Exit() {
        // Debug.Log("Exiting Melee Attack State");
        _animController.ResetTrigger("Attack");
        HitBoxesActive = false;
    }

    private void SetHitBoxesActive(bool active) {
        foreach(Collider hitBox in _hitBoxes) {
            hitBox.gameObject.SetActive(active);
        }
    }
}
