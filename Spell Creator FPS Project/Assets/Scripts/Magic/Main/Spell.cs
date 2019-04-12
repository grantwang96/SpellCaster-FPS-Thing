using System;
using UnityEngine;

[System.Serializable]
public class Spell {
    public string Name { get; private set; }

    [SerializeField] private Spell_CastingMethod _castingMethod;
    public Spell_CastingMethod CastingMethod {
        get { return _castingMethod; }
    }
    [SerializeField] private Spell_Effect[] _effects;
    public Spell_Effect[] Effects { get { return _effects; } }
    [SerializeField] private SpellModifier[] _spellModifiers;
    public SpellModifier[] SpellModifiers { get { return _spellModifiers; } }

    public int ManaCost { get; private set; }
    public float MaxChargeTime { get; private set; }
    public float IntervalTime { get; private set; }

    public int Power { get; private set; }

    public Spell(Spell_CastingMethod castingMethod, Spell_Effect[] effects, SpellModifier[] spellModifiers = null) {
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

    public Spell(Spell_CastingMethod castingMethod, Spell_Effect[] effects, float maxChargeTime, float intervalTime, int power, string name, SpellModifier[] spellModifiers = null) {
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

    public StorableSpell(string castingMethodId, string[] spellEffectIds, string[] spellModifierIds) {
        _castingMethodId = castingMethodId;
        _spellEffectIds = spellEffectIds;
        _spellModifierIds = spellModifierIds;
    }
}
