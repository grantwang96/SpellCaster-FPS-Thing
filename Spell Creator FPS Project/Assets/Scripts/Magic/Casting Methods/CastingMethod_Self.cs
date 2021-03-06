﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Casting Method/Self")]
public class CastingMethod_Self : Spell_CastingMethod {

    protected override void CastSpell(ISpellCaster caster, Spell spell) {
        List<Effect> effects = new List<Effect>(spell.Effects);
        for (int i = 0; i < spell.Effects.Length; i++) {
            Damageable damageable = caster.Damageable;
            if (damageable != null) {
                float powerScale = GetTotalPowerScale(caster.ActiveSpell, spell);
                spell.Effects[i].TriggerEffect(caster.Damageable, powerScale, caster.Damageable.Body.position, damageable, effects);
            }
        }
    }
}