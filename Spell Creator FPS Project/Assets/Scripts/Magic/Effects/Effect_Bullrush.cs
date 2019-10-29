using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Effect/Bullrush")]
public class Effect_Bullrush : Effect {

    [SerializeField] private float _overrideDrag;

    public override void TriggerEffect(Damageable caster, float powerScale, List<Effect> additionalEffects = null) {
        Vector3 velocity = caster.Body.forward * _basePower * powerScale;
        caster.AddForce(velocity, _overrideDrag);
    }

    public override void TriggerEffect(Damageable caster, float powerScale, Vector3 position, Damageable damageable = null, List<Effect> additionalEffects = null) {
        if(damageable == null) {
            return;
        }
        Vector3 velocity = damageable.Body.forward * _basePower * powerScale;
        damageable.AddForce(velocity, _overrideDrag);
    }

    public override void TriggerEffect(Damageable caster, Vector3 velocity, float powerScale, Vector3 position, Damageable damageable = null, List<Effect> additionalEffects = null) {
        TriggerEffect(caster, powerScale, position, damageable, additionalEffects);
    }
}
