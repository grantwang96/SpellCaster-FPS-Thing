using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the base class for how spells are cast
/// </summary>
public abstract class Spell_CastingMethod : ScriptableObject {

    /// <summary>
    /// Called when the spell is first cast
    /// </summary>
    /// <param name="caster"></param>
    public abstract void OnStartCast(ISpellCaster caster, Spell_Effect[] effects);

    /// <summary>
    /// Called while the spell is held
    /// </summary>
    /// <param name="caster"></param>
    public abstract void OnHoldCast(ISpellCaster caster, Spell_Effect[] effects);

    /// <summary>
    /// Called when the spell is released
    /// </summary>
    /// <param name="caster"></param>
    public abstract void OnEndCast(ISpellCaster caster, Spell_Effect[] effects);
}
