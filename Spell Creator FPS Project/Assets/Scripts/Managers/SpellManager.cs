using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager class that loads and handles spell generation
/// </summary>
public class SpellManager : MonoBehaviour {

    public static SpellManager Instance;

    [SerializeField] private Spell_CastingMethod[] _castingMethods;
    public int castingMethodsLength { get { return _castingMethods.Length; } }
    [SerializeField] private Effect[] _spellEffects;
    public int spellEffectsLength { get { return _spellEffects.Length; } }
    [SerializeField] private SpellModifier[] _spellModifiers;
    public int spellModifiersLength { get { return _spellModifiers.Length; } }

    [SerializeField] private SpellBook spellBookPrefab;

    private void Awake() {
        Instance = this;
    }

    public Spell GenerateRandomSpell() {
        int index = Random.Range(0, castingMethodsLength);
        Spell_CastingMethod castingMethod = _castingMethods[index];
        index = Random.Range(0, spellEffectsLength);
        Effect[] effects = _spellEffects;
        SpellModifier[] spellModifiers = _spellModifiers;
        return new Spell(castingMethod, effects, spellModifiers);
    }

    public Spell GenerateSpell(Spell_CastingMethod castingMethod, Effect[] effects, SpellModifier[] spellModifiers) {
        Spell newSpell = new Spell(castingMethod, effects, spellModifiers);
        return newSpell;
    }

    public void GenerateSpellBook(Spell spell, Vector3 location, Quaternion rotation) {
        // create a spellbook with this spell inside
        SpellBook newSpellBook = Instantiate(spellBookPrefab, location, rotation);
    }

    // TODO: when chests are implemented, use this to build a spell based on ID
    public Spell GenerateSpellFromChestId(string id) {
        return GenerateRandomSpell();
    }
}
