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
    [SerializeField] private float _powerScale;
    [SerializeField] private Element _element;

    public override void TriggerEffect(Damageable caster, int power, List<Effect> effects = null) {
        caster?.TakeDamage(power, _element);
    }

    public override void TriggerEffect(Damageable caster, int power, Vector3 position, Damageable damageable = null, List<Effect> effects = null) {
        damageable?.TakeDamage(power, _element);
    }

    public override void TriggerEffect(Damageable caster, Vector3 velocity, int power, Vector3 position, Damageable damageable = null, List<Effect> effects = null) {
        if (caster == damageable) { return; }
        Vector3 vel = velocity.normalized * _force;
        vel.y = _upwardForce;
        damageable?.TakeDamage(power, _element, vel * _powerScale);
    }
}
