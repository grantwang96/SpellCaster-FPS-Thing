using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Each attack state should reference one attack
/// </summary>
public abstract class AttackState : BrainState {
    
    [SerializeField] protected float _duration;
    [SerializeField] protected NPCVision _npcVision;
}
