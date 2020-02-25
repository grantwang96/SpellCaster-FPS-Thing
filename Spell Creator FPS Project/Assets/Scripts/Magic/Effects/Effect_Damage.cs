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
    [SerializeField] private Element _element;

    public override void TriggerEffect(Damageable caster, float powerScale, List<Effect> effects = null) {
        caster?.TakeDamage(caster, GetTotalPower(powerScale), _element);
    }

    public override void TriggerEffect(Damageable caster, float powerScale, Vector3 position, Damageable damageable = null, List<Effect> effects = null) {
        damageable?.TakeDamage(caster, GetTotalPower(powerScale), _element);
    }

    public override void TriggerEffect(Damageable caster, Vector3 velocity, float powerScale, Vector3 position, Damageable damageable = null, List<Effect> effects = null) {
        if (caster == damageable) { return; }
        Vector3 vel = velocity.normalized * _force;
        vel.y = _upwardForce;
        int totalPower = GetTotalPower(powerScale);
        damageable?.TakeDamage(caster, totalPower, _element);
        damageable?.AddForce(vel, totalPower);
    }
}
