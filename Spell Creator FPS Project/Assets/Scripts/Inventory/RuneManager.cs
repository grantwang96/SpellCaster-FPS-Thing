using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneManager : MonoBehaviour {

    public static RuneManager Instance { get; protected set; }

    [SerializeField] private string _spellCastingMethodResourcesLocation;
    private Dictionary<string, Spell_CastingMethod> _spellCastingMethods = new Dictionary<string, Spell_CastingMethod>();
    [SerializeField] private string _spellEffectsResourcesLocation;
    private Dictionary<string, Spell_Effect> _spellEffects = new Dictionary<string, Spell_Effect>();
    [SerializeField] private string _spellModifiersResourcesLocation;
    private Dictionary<string, SpellModifier> _spellModifiers = new Dictionary<string, SpellModifier>();

    private void Awake() {
        if(Instance != null && Instance != this) {
            Debug.LogError($"RUNE MANAGER INSTANCE ALREADY CREATED!");
            return;
        }
        Instance = this;
        LoadAllSpellEffects();
    }

    private void OnDestroy() {
        if(Instance != null && Instance == this) {
            Instance = null;
        }
    }

    private void LoadAllSpellEffects() {
        Object[] spellEffects = Resources.LoadAll(_spellEffectsResourcesLocation, typeof(Spell_Effect));
        foreach(var obj in spellEffects) {
            Spell_Effect effect = obj as Spell_Effect;
            if (!effect) {
                Debug.LogError($"Object {obj} is not a spell effect! Skipping...");
                continue;
            }
            if (_spellEffects.ContainsKey(effect.Id)) {
                Debug.LogError($"Spell Effect dictionary already contains Key ({effect.Id}) with Value ({_spellEffects[effect.Id]})! Skipping...");
                continue;
            }
            Debug.Log($"Adding spell effect ({effect}) with id ({effect.Id})");
            _spellEffects.Add(effect.Id, effect);
        }
    }
}
