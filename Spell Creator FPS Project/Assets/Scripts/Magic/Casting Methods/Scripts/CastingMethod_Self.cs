using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Casting Method/Self")]
public class CastingMethod_Self : Spell_CastingMethod {

    protected override void ApplyEffects(ISpellCaster caster, Spell_Effect[] effects) {
        for (int i = 0; i < effects.Length; i++) {
            IDamageable damageable = caster.Damageable;
            if (damageable != null) {
                effects[i].TriggerEffect(damageable, caster);
            }
        }
    }
}
