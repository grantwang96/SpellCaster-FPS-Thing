using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Effect/Heal")]
public class Effect_Heal : Effect {

    // visual effect prefab

    public override void TriggerEffect(Damageable caster, int power, List<Effect> additionalEffects = null) {
        caster?.TakeDamage(-power);
    }

    public override void TriggerEffect(Damageable caster, int power, Vector3 position, Damageable damageable = null, List<Effect> additionalEffects = null) {
        damageable.TakeDamage(-power);
    }

    public override void TriggerEffect(Damageable caster, Vector3 velocity, int power, Vector3 position, Damageable damageable = null, List<Effect> additionalEffects = null) {
        damageable?.TakeDamage(-power);
    }
}
