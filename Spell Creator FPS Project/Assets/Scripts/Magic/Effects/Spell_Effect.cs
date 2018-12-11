using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// What happens when the spell hits
/// </summary>
public abstract class Spell_Effect : ScriptableObject {

    [SerializeField] private int _manaCost; // how much will this effect cost

    /// <summary>
    /// Applies effect to given damageable
    /// </summary>
    /// <param name="damageable"></param>
    public abstract void TriggerEffect(IDamageable damageable, ISpellCaster caster);
}
