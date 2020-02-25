using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Effect/Add Status Effect")]
public class Effect_AddStatusEffect : Effect {

    [SerializeField] private StatusEffect _statusEffect;

    public override void TriggerEffect(Damageable caster, float powerScale, List<Effect> effects = null) {
        caster.TakeDamage(caster, GetTotalPower(powerScale), Element.Neutral, Vector3.zero, _statusEffect);
    }

    public override void TriggerEffect(Damageable caster, float powerScale, Vector3 position, Damageable damageable = null, List<Effect> effects = null) {
        damageable?.TakeDamage(caster, GetTotalPower(powerScale), Element.Neutral, Vector3.zero, _statusEffect);
    }

    public override void TriggerEffect(
        Damageable caster,
        Vector3 velocity,
        float powerScale,
        Vector3 position,
        Damageable damageable = null,
        List<Effect> effects = null)
    {
        damageable?.TakeDamage(caster, GetTotalPower(powerScale), Element.Neutral, Vector3.zero, _statusEffect);
    }
}
