using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Runs for the duration of an attack animation
/// </summary>
public abstract class AttackState : BrainState {
    
    [SerializeField] protected float _duration;
}
