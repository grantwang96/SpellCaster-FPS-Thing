using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic damage effect
/// </summary>
[CreateAssetMenu(menuName = "Spell Effect/Damage")]
public class Effect_Damage : Spell_Effect {
    
    [SerializeField] private float _upwardForce;
    [SerializeField] private float _force;

    public override void TriggerEffect(ISpellCaster caster, int power) {
        
    }

    public override void TriggerEffect(IDamageable damageable, ISpellCaster caster, int power) {
        Debug.Log("Applied damage: " + power);
        damageable.TakeDamage(power);
    }

    public override void TriggerEffect(IDamageable damageable, ISpellCaster caster, Vector3 velocity, int power) {
        Debug.Log("Applied damage: " + power);
        velocity.y += _upwardForce;
        damageable.TakeDamage(power, velocity * power);
    }
}
