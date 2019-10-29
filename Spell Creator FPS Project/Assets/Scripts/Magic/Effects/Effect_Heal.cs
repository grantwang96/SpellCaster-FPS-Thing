using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Effect/Heal")]
public class Effect_Heal : Effect {

    // visual effect prefab

    public override void TriggerEffect(Damageable caster, float powerScale, List<Effect> additionalEffects = null) {
        caster?.TakeDamage(-GetTotalPower(powerScale), Element.Healing);
    }

    public override void TriggerEffect(Damageable caster, float powerScale, Vector3 position, Damageable damageable = null, List<Effect> additionalEffects = null) {
        damageable.TakeDamage(-GetTotalPower(powerScale), Element.Healing);
    }

    public override void TriggerEffect(Damageable caster, Vector3 velocity, float powerScale, Vector3 position, Damageable damageable = null, List<Effect> additionalEffects = null) {
        damageable?.TakeDamage(-GetTotalPower(powerScale), Element.Healing);
    }
}
