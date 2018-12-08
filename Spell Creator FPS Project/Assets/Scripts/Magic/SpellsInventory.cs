using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellsInventory : MonoBehaviour {

    [SerializeField] private Spell[] spellsList;
    public Spell[] SpellsList { get { return spellsList; } }
    public void SetNewSpellsList(Spell[] newSpellsList) {
        spellsList = newSpellsList;
    }
}

public class Spell {

    public Spell_CastingMethod CastingMethod { get; private set; }
    public Spell_Effect[] Effects { get; private set; }

    public int ManaCost { get; private set; }
    
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
