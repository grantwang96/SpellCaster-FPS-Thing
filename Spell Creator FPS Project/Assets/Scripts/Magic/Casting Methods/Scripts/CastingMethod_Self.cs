using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Casting Method/Self")]
public class CastingMethod_Self : Spell_CastingMethod {

    protected override void CastSpell(ISpellCaster caster, Spell spell) {
        for (int i = 0; i < spell.Effects.Length; i++) {
            IDamageable damageable = caster.Damageable;
            if (damageable != null) {
                spell.Effects[i].TriggerEffect(damageable, caster, spell.Effects[i].BasePower);
            }
        }
    }
}
