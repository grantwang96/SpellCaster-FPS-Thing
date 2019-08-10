﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Applies an effect(could be spell, weapon, or projectile)
/// </summary>
public abstract class Effect : ScriptableObject, IInventoryStorable {

    [SerializeField] protected InventoryItemType _itemType;
    public InventoryItemType ItemType => _itemType;
    [SerializeField] protected string _id;
    public string Id => _id;
    [SerializeField] protected Sprite _smallIcon;
    public Sprite SmallIcon => _smallIcon;
    [SerializeField] protected Sprite _largeIcon;
    public Sprite LargeIcon => _largeIcon;
    [SerializeField] protected string _name;
    public string Name => _name;
    [SerializeField] protected string _shortDescription;
    public string ShortDescription => _shortDescription;
    [SerializeField] protected string _longDescription;
    public string LongDescription => _longDescription;

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

    public abstract void TriggerEffect(Damageable caster, int power, List<Effect> additionalEffects = null);
    /// <summary>
    /// Applies effect to given damageable
    /// </summary>
    /// <param name="damageable"></param>
    public abstract void TriggerEffect(Damageable caster, int power, Vector3 position, Damageable damageable = null, List<Effect> additionalEffects = null);
    public abstract void TriggerEffect(Damageable caster, Vector3 velocity, int power, Vector3 position, Damageable damageable = null, List<Effect> additionalEffects = null);

    public virtual void TriggerEffect(Damageable caster, int power, Vector3 position, Collider collider, List<Effect> additionalEffects = null) {

    }
}
