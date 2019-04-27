using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackState : AttackState {

    [SerializeField] private BrainState _onTargetInRangeState; // transition to this state if target is within this range
    [SerializeField] private BrainState _onTargetOutOfRangeState; // transition to this state if target is out of range
    
    [SerializeField] private string _attackName;
    [SerializeField] private int _attackComboIndex;

    // [SerializeField] private NPCAnimController _animController;
    [SerializeField] private NPCCombat _npcCombat;
    [SerializeField] private NPCMoveController _moveController;
    
    private void Awake() {
        _npcCombat.OnAttackStarted += OnAttackStarted;
        _npcCombat.OnAttackFinished += OnAttackFinished;
    }

    public override void Enter(BrainState overrideBrainState = null) {
        base.Enter(overrideBrainState);
        if (_attackComboIndex >= _npcBehaviour.Blueprint.AttackComboMax) { _attackComboIndex = 0; }
        _attackName = _npcCombat.NextAttack();
        if (!_npcCombat.Attack(_attackName)) {
            _npcBehaviour.ChangeBrainState(_onTargetOutOfRangeState);
        }
        _moveController.Stop();
    }

    private void OnAttackStarted() {
        // on attack commenced stuff
    }

    private void OnAttackFinished() {
        if (_npcBehaviour.Blueprint.CanAttack(_npcBehaviour, _npcVision.CurrentTarget)) {
            _attackComboIndex++;
            _npcBehaviour.ChangeBrainState(_onTargetInRangeState);
            return;
        }
        _attackComboIndex = 0;
        _npcBehaviour.ChangeBrainState(_onTargetOutOfRangeState);
    }

    private void OnDestroy() {
        _npcCombat.OnAttackStarted -= OnAttackStarted;
        _npcCombat.OnAttackFinished -= OnAttackFinished;
    }
}
