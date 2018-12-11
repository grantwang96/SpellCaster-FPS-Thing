using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour {

    public static SpellManager Instance;

    [SerializeField] private Spell_CastingMethod[] _castingMethods;
    [SerializeField] private Spell_Effect[] _spellEffects;

    private void Awake() {
        Instance = this;
    }

    public Spell GenerateSpell(Spell_CastingMethod castingMethod, Spell_Effect[] effects) {
        Spell newSpell = new Spell(castingMethod, effects);
        return newSpell;
    }
}
