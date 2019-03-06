using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// What happens when the spell hits
/// </summary>
public abstract class Spell_Effect : SpellComponent {
    
    [SerializeField] private int _manaCost; // how much will this effect cost
    public int ManaCost {
        get {
            return _manaCost;
        }
    }
    [SerializeField] private float _chargeTime;
    public float ChargeTime { get {
            return _chargeTime;
        } }
    [SerializeField] private float _intervalTime;
    public float IntervalTime {
        get {
            return _intervalTime;
        }
    }

    [SerializeField] private int _basePower;
    public int BasePower { get { return _basePower; } }

    public abstract void TriggerEffect(ISpellCaster caster, int power);
    /// <summary>
    /// Applies effect to given damageable
    /// </summary>
    /// <param name="damageable"></param>
    public abstract void TriggerEffect(ISpellCaster caster, int power, Damageable damageable = null);
    public abstract void TriggerEffect(ISpellCaster caster, Vector3 velocity, int power, Damageable damageable = null);
}
