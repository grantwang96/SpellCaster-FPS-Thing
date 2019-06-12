﻿using System;
using UnityEngine;

[System.Serializable]
public class Spell {
    public string Name { get; private set; }
    public string InstanceId { get; private set; }
    public bool Favorite { get; private set; }

    [SerializeField] private Spell_CastingMethod _castingMethod;
    public Spell_CastingMethod CastingMethod {
        get { return _castingMethod; }
    }
    [SerializeField] private Effect[] _effects;
    public Effect[] Effects { get { return _effects; } }
    [SerializeField] private SpellModifier[] _spellModifiers;
    public SpellModifier[] SpellModifiers { get { return _spellModifiers; } }

    public int ManaCost { get; private set; }
    public float MaxChargeTime { get; private set; }
    public float IntervalTime { get; private set; }

    public int Power { get; private set; }

    public Spell(Spell_CastingMethod castingMethod, Effect[] effects, SpellModifier[] spellModifiers = null) {
        
        InstanceId = StorableSpell.GenerateInstanceId();
        // TODO: GENERATE DEFAULT NAME IF NAME ISN'T GIVEN
        Name = $"NoNameSadLife_{StringGenerator.RandomString(5)}";

        _castingMethod = castingMethod;
        _effects = effects;
        _spellModifiers = spellModifiers;
        ManaCost = _castingMethod.ManaCost;
        IntervalTime = _castingMethod.IntervalTime;
        for(int i = 0; i < _effects.Length; i++) {
            IntervalTime += _effects[i]?.IntervalTime ?? 0;
            ManaCost += _effects[i]?.ManaCost ?? 0;
            Power += _effects[i]?.BasePower ?? 0;
        }
        if(spellModifiers != null) {
            for (int i = 0; i < spellModifiers.Length; i++) {
                _spellModifiers[i]?.SetupSpell(this);
            }
        }
        IntervalTime = Mathf.Clamp(IntervalTime, 0f, 100f);
        MaxChargeTime = 3f;
    }

    public Spell(Spell_CastingMethod castingMethod, Effect[] effects, float maxChargeTime, float intervalTime, int power, string name, SpellModifier[] spellModifiers = null) {
        _castingMethod = castingMethod;
        _effects = effects;
        _spellModifiers = spellModifiers;
        ManaCost = _castingMethod.ManaCost;
        for (int i = 0; i < _effects.Length; i++) {
            ManaCost += _effects[i]?.ManaCost ?? 0;
        }
        if(spellModifiers != null) {
            for (int i = 0; i < spellModifiers.Length; i++) {
                _spellModifiers[i]?.SetupSpell(this);
            }
        }
        MaxChargeTime = maxChargeTime;
        IntervalTime = intervalTime;
        Power = power;
        Name = name;
    }

    public bool OnStartCastSpell(ISpellCaster caster) {
        return CastingMethod.OnStartCast(caster, this);
    }

    public bool OnHoldCastSpell(ISpellCaster caster) {
        return CastingMethod.OnHoldCast(caster, this);
    }

    public bool OnEndCastSpell(ISpellCaster caster) {
        return CastingMethod.OnEndCast(caster, this);
    }
}

/// <summary>
/// Struct that processes actively casted spells
/// </summary>
public class ActiveSpell {

    public float holdTime;
    public float holdIntervalTime;
    public float interval;
    public float maxHoldTime;
    public int baseManaCost;
    public int totalManaCost;
    public Vector3 initialHitPoint;
}

/// <summary>
/// This is the inventory view version of the spell and should be persisted
/// </summary>
[System.Serializable]
public class StorableSpell {
    
    private string _castingMethodId;
    private string[] _spellEffectIds;
    private string[] _spellModifierIds;

    public string InstanceId { get; private set; }
    public string Name { get; private set; }
    public bool Favorite { get; private set; }

    public StorableSpell(string castingMethodId, string[] spellEffectIds, string[] spellModifierIds, bool favorite = false) {
        _castingMethodId = castingMethodId;
        _spellEffectIds = spellEffectIds;
        _spellModifierIds = spellModifierIds;

        InstanceId = GenerateInstanceId();
        Name = ""; // TODO: GENERATE DEFAULT NAME FROM SPELL COMPONENTS
        Favorite = favorite;
    }

    public void SetName(string newName) {
        Name = newName;
    }

    public void SetFavorite(bool favorite) {
        Favorite = favorite;
    }

    public static string GenerateInstanceId() {
        return $"{GameplayValues.Magic.StorableSpellInstanceIdPrefix}_{StringGenerator.RandomString(GameplayValues.Magic.StorableSpellInstanceIdSize)}";
    }

    public Spell ConvertToSpell() {
        IInventoryStorable storable = InventoryRegistry.Instance.GetItemById(_castingMethodId);
        Spell_CastingMethod castingMethod = storable as Spell_CastingMethod;
        if (castingMethod == null) {
            Debug.LogError($"Unable to retrieve Casting Method object from ID: {_castingMethodId}");
            return null;
        }
        Effect[] effects = new Effect[_spellModifierIds.Length];
        for(int i = 0; i < _spellEffectIds.Length; i++) {
            storable = InventoryRegistry.Instance.GetItemById(_spellEffectIds[i]);
            Effect effect = storable as Effect;
            if(effect == null) {
                Debug.LogError($"Unable to retrieve Effect object from ID: {_spellEffectIds[i]}");
                return null;
            }
            effects[i] = effect;
        }
        SpellModifier[] modifiers = new SpellModifier[_spellModifierIds.Length];
        for(int i = 0; i < _spellModifierIds.Length; i++) {
            storable = InventoryRegistry.Instance.GetItemById(_spellModifierIds[i]);
            SpellModifier modifier = storable as SpellModifier;
            if(modifier == null) {
                Debug.LogError($"Unable to retrieve Spell Modifier object from ID: {_spellModifierIds[i]}");
                return null;
            }
            modifiers[i] = modifier;
        }
        return new Spell(castingMethod, effects, modifiers); ;
    }
}
