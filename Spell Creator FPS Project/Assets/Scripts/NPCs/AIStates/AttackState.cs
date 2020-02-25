using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Each attack state should reference one attack
/// </summary>
public abstract class AttackState : BrainState {

    [SerializeField] protected float _attackRange;
    [SerializeField] protected NPCMoveController _moveController;

    public override void Enter(BrainState overrideNextState = null, float duration = 0, bool isChild = false) {
        base.Enter(overrideNextState, duration);

        if (!isChild) {
            _npcBehaviour.Damageable.OnKnockback += OnEnterHitStun;
            _npcBehaviour.Damageable.OnStun += OnEnterHitStun;
        }
    }

    protected bool TargetWithinRange(Vector3 position) {
        float distance = Vector3.Distance(_npcBehaviour.GetBodyPosition(), position);
        return distance <= _attackRange;
    }

    public override void Exit() {
        base.Exit();

        _npcBehaviour.Damageable.OnKnockback -= OnEnterHitStun;
        _npcBehaviour.Damageable.OnStun -= OnEnterHitStun;
    }
}
