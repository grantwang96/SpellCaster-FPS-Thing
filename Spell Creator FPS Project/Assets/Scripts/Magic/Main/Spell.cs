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

    public int ManaCost { get; private set; }
    public float MaxChargeTime { get; private set; }
    public float IntervalTime { get; private set; }

    public int Power { get; private set; }

    public Spell(Spell_CastingMethod castingMethod, Spell_Effect[] effects) {
        _castingMethod = castingMethod;
        _effects = effects;
        ManaCost = _castingMethod.ManaCost;
        for(int i = 0; i < _effects.Length; i++) {
            ManaCost += _effects[i].ManaCost;
        }
        IntervalTime = .25f;
        MaxChargeTime = 3f;
    }

    public Spell(Spell_CastingMethod castingMethod, Spell_Effect[] effects, float maxChargeTime, float intervalTime, int power, string name) {
        _castingMethod = castingMethod;
        _effects = effects;
        ManaCost = _castingMethod.ManaCost;
        for (int i = 0; i < _effects.Length; i++) {
            ManaCost += _effects[i].ManaCost;
        }
        MaxChargeTime = maxChargeTime;
        IntervalTime = intervalTime;
        Power = power;
        Name = name;
    }

    public void OnStartCastSpell(ISpellCaster caster) {
        CastingMethod.OnStartCast(caster, Effects);
    }

    public void OnHoldCastSpell(ISpellCaster caster) {
        CastingMethod.OnHoldCast(caster, Effects);
    }

    public void OnEndCastSpell(ISpellCaster caster) {
        CastingMethod.OnEndCast(caster, Effects);
    }
}

/// <summary>
/// Struct that processes actively casted spells
/// </summary>
public struct ActiveSpell {

    public float holdTime;
    public float holdIntervalTime;
    public float interval;
    public float maxHoldTime;
}
