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

    public override void TriggerEffect(ISpellCaster caster, int power, Spell castedSpell) {
        
    }

    public override void TriggerEffect(ISpellCaster caster, int power, Spell castedSpell, Damageable damageable = null) {
        if(caster.Damageable == damageable) { return; }
        Debug.Log("Applied damage: " + power);
        damageable?.TakeDamage(power);
    }

    public override void TriggerEffect(ISpellCaster caster, Vector3 velocity, int power, Spell castedSpell, Vector3 position, Damageable damageable = null) {
        if (caster.Damageable == damageable) { return; }
        Debug.Log("Applied damage: " + power);
        velocity.y += _upwardForce;
        damageable?.TakeDamage(power, velocity * power);
    }
}
