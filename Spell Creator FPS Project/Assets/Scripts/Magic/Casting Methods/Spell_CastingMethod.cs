using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the base class for how spells are cast
/// </summary>
public abstract class Spell_CastingMethod : ScriptableObject {

    [SerializeField] protected int manaCost;
    public int ManaCost { get { return manaCost; } }
    public enum SpellTiming {
        Instant, Continuous
, Charge
    }
    [SerializeField] protected SpellTiming[] spellTiming;

    /// <summary>
    /// Called when the spell is first cast
    /// </summary>
    /// <param name="caster"></param>
    public abstract bool OnStartCast(ISpellCaster caster, Spell_Effect[] effects); // returns if spell is successfully cast

    /// <summary>
    /// Called while the spell is held
    /// </summary>
    /// <param name="caster"></param>
    public abstract bool OnHoldCast(ISpellCaster caster, Spell_Effect[] effects); // returns if spell is successfully cast

    /// <summary>
    /// Called when the spell is released
    /// </summary>
    /// <param name="caster"></param>
    public abstract bool OnEndCast(ISpellCaster caster, Spell_Effect[] effects); // returns if spell is successfully cast
}
