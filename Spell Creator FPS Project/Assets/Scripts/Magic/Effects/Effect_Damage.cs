using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic damage effect
/// </summary>
[CreateAssetMenu(menuName = "Spell Effect/Damage")]
public class Effect_Damage : Effect {
    
    [SerializeField] private float _upwardForce;
    [SerializeField] private float _force;

    public override void TriggerEffect(Damageable caster, int power, List<Effect> effects = null) {
        
    }

    public override void TriggerEffect(Damageable caster, int power, Vector3 position, Damageable damageable = null, List<Effect> effects = null) {
        if(caster == damageable) { return; }
        Debug.Log("Applied damage: " + power);
        damageable?.TakeDamage(power);
    }

    public override void TriggerEffect(Damageable caster, Vector3 velocity, int power, Vector3 position, Damageable damageable = null, List<Effect> effects = null) {
        if (caster == damageable) { return; }
        velocity.y += _upwardForce;
        damageable?.TakeDamage(power, velocity * power);
    }
}
