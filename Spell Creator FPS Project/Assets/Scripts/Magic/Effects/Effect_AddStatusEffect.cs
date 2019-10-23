﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Effect/Add Status Effect")]
public class Effect_AddStatusEffect : Effect {

    [SerializeField] private StatusEffect _statusEffect;

    public override void TriggerEffect(Damageable caster, int power, List<Effect> effects = null) {
        caster.TakeDamage(power, Element.Neutral, _statusEffect);
    }

    public override void TriggerEffect(Damageable caster, int power, Vector3 position, Damageable damageable = null, List<Effect> effects = null) {
        damageable.TakeDamage(power, Element.Neutral, _statusEffect);
    }

    public override void TriggerEffect(
        Damageable caster,
        Vector3 velocity,
        int power,
        Vector3 position,
        Damageable damageable = null,
        List<Effect> effects = null)
        {
        damageable.TakeDamage(power, Element.Neutral, _statusEffect);
    }
}
