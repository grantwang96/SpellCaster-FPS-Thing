using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour {

    public static SpellManager Instance;

    [SerializeField] private Spell_CastingMethod[] _castingMethods;
    public int castingMethodsLength { get { return _castingMethods.Length; } }

    [SerializeField] private Spell_Effect[] _spellEffects;
    public int spellEffectsLength { get { return _spellEffects.Length; } }

    [SerializeField] private SpellBook spellBookPrefab;

    private void Awake() {
        Instance = this;
    }

    public Spell GenerateRandomSpell() {
        int index = Random.Range(0, castingMethodsLength);
        Spell_CastingMethod castingMethod = _castingMethods[index];
        index = Random.Range(0, spellEffectsLength);
        Spell_Effect[] effects = new Spell_Effect[1];
        Spell_Effect spellEffect = _spellEffects[index];
        effects[0] = spellEffect;
        return new Spell(castingMethod, effects);
    }

    public Spell GenerateSpell(Spell_CastingMethod castingMethod, Spell_Effect[] effects) {
        Spell newSpell = new Spell(castingMethod, effects);
        return newSpell;
    }

    public void GenerateSpellBook(Spell spell, Vector3 location, Quaternion rotation) {
        // create a spellbook with this spell inside
        SpellBook newSpellBook = Instantiate(spellBookPrefab, location, rotation);
    }
}
