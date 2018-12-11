using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Casting Method/Self")]
public class CastingMethod_Self : Spell_CastingMethod {
    
    public override bool OnStartCast(ISpellCaster caster, Spell_Effect[] effects) {
        if(ArrayHelper.Contains(spellTiming, SpellTiming.Instant)) {
            ApplyEffects(caster, effects);
            return true;
        }
        return false;
    }

    public override bool OnHoldCast(ISpellCaster caster, Spell_Effect[] effects) {
        if (ArrayHelper.Contains(spellTiming, SpellTiming.Continuous)) {
            if(Mathf.Approximately(caster.ActiveSpell.holdIntervalTime, 0f)) {
                ApplyEffects(caster, effects);
                return true;
            }
        }
        return false;
    }

    public override bool OnEndCast(ISpellCaster caster, Spell_Effect[] effects) {
        if (ArrayHelper.Contains(spellTiming, SpellTiming.Charge)) {
            ApplyEffects(caster, effects);
            return true;
        }
        return false;
    }
}
