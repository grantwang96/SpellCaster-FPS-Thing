using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Each attack state should reference one attack
/// </summary>
public abstract class AttackState : BrainState {
    
    [SerializeField] protected NPCVision _npcVision;
    [SerializeField] protected float _attackRange;
    
    [SerializeField] protected NPCMoveController _moveController;

    protected bool TargetWithinRange(Vector3 position) {
        float distance = Vector3.Distance(_npcBehaviour.GetBodyPosition(), position);
        return distance <= _attackRange;
    }
}
