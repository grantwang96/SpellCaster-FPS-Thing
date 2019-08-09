using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Casting Method/Self")]
public class CastingMethod_Self : Spell_CastingMethod {

    protected override void CastSpell(ISpellCaster caster, Spell spell) {
        List<Effect> effects = new List<Effect>(spell.Effects);
        for (int i = 0; i < spell.Effects.Length; i++) {
            Damageable damageable = caster.Damageable;
            if (damageable != null) {
                int power = GetPower(caster.ActiveSpell, spell.Effects[i].BasePower);
                spell.Effects[i].TriggerEffect(caster.Damageable, power, damageable, effects);
            }
        }
    }

    private int GetPower(ActiveSpell spell, int power) {
        float multiplier = spell.holdTime / spell.interval;
        int result = Mathf.FloorToInt(power * multiplier);
        return result > power ? result : power;
    }
}