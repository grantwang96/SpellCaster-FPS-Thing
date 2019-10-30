using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Each attack state should reference one attack
/// </summary>
public abstract class AttackState : BrainState {
    
    [SerializeField] protected NPCVision _npcVision;
    [SerializeField] protected string _attackName; // this should reflect the name of the actual animation state

    [SerializeField] protected NPCAnimController _animController;
    [SerializeField] protected NPCMoveController _moveController;
}
