using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the base class for how spells are cast
/// </summary>
public abstract class Spell_CastingMethod : SpellComponent {
    
    [SerializeField] protected int manaCost;
    public virtual int ManaCost { get { return manaCost; } }
    public enum SpellTiming {
        Instant, Continuous, Charge
    }
    [SerializeField] protected SpellTiming[] spellTiming;

    /// <summary>
    /// Called when the spell is first cast
    /// </summary>
    /// <param name="caster"></param>
    public virtual bool OnStartCast(ISpellCaster caster, Spell spell) {
        if (ArrayHelper.Contains(spellTiming, SpellTiming.Instant)) {
            CastSpell(caster, spell);
            return true;
        }
        return false;
    }// returns if spell is successfully cast

    /// <summary>
    /// Called while the spell is held
    /// </summary>
    /// <param name="caster"></param>
    public virtual bool OnHoldCast(ISpellCaster caster, Spell spell) {
        if (ArrayHelper.Contains(spellTiming, SpellTiming.Continuous)) {
            if (Mathf.Approximately(caster.ActiveSpell.holdIntervalTime, 0f)) {
                CastSpell(caster, spell);
                return true;
            }
        }
        return false;
    }// returns if spell is successfully cast

    /// <summary>
    /// Called when the spell is released
    /// </summary>
    /// <param name="caster"></param>
    public virtual bool OnEndCast(ISpellCaster caster, Spell spell) {
        if (ArrayHelper.Contains(spellTiming, SpellTiming.Charge)) {
            CastSpell(caster, spell);
            return true;
        }
        return false;
    } // returns if spell is successfully cast

    protected abstract void CastSpell(ISpellCaster caster, Spell spell);
}
