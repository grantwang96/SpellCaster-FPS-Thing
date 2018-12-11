using System;

[System.Serializable]
public class Spell {

    public Spell_CastingMethod CastingMethod { get; private set; }
    public Spell_Effect[] Effects { get; private set; }

    public int ManaCost { get; private set; }
    public float maxChargeTime { get; private set; }
    public float intervalTime { get; private set; }

    public Spell(Spell_CastingMethod castingMethod, Spell_Effect[] effects) {
        CastingMethod = castingMethod;
        Effects = effects;
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
