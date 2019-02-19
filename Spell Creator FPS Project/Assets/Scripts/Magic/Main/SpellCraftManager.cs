using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCraftManager {

    [SerializeField] private Spell_CastingMethod _loadedCastingMethod;
    public Spell_CastingMethod LoadedCastingMethod {
        get {
            return _loadedCastingMethod;
        }
    }
    [SerializeField] private List<Spell_Effect> _loadedSpellEffects = new List<Spell_Effect>();
    public List<Spell_Effect> LoadedSpellEffects {
        get {
            return _loadedSpellEffects;
        }
    }
    [SerializeField] private List<SpellModifier> _loadedSpellModifiers = new List<SpellModifier>();
    public List<SpellModifier> LoadedSpellModifiers {
        get {
            return _loadedSpellModifiers;
        }
    }

    [SerializeField] private int _loadedSpellManaCost;

    public void SetCastingMethod(Spell_CastingMethod castingMethod) {
        _loadedCastingMethod = castingMethod;
    }

    public void AddSpellEffect(Spell_Effect spellEffect) {
        if (!_loadedSpellEffects.Contains(spellEffect)) {
            _loadedSpellEffects.Add(spellEffect);
            UpdateSpellManaCost();
        }
    }

    public void AddSpellModifier(SpellModifier spellModifier) {
        if (!_loadedSpellModifiers.Contains(spellModifier)) {
            _loadedSpellModifiers.Add(spellModifier);
        }
    }

    public void RemoveComponentFromSpell(string itemId) {
        IInventoryStorable storable = InventoryRegistry.Instance.GetItemById(itemId);
        Spell_CastingMethod castingMethod = storable as Spell_CastingMethod;
        if (castingMethod != null) {
            _loadedCastingMethod = null;
            return;
        }
        Spell_Effect spellEffect = storable as Spell_Effect;
        if (spellEffect != null) {
            _loadedSpellEffects.Remove(spellEffect);
            UpdateSpellManaCost();
            return;
        }
        SpellModifier spellModifier = storable as SpellModifier;
        if (spellModifier != null) {
            _loadedSpellModifiers.Remove(spellModifier);
            return;
        }
    }

    private void UpdateSpellManaCost() {
        _loadedSpellManaCost = 0;
        for(int i = 0; i < _loadedSpellEffects.Count; i++) {
            _loadedSpellManaCost += _loadedSpellEffects[i].ManaCost;
        }
    }

    public void GenerateSpell() {
        if(_loadedCastingMethod == null || _loadedSpellEffects.Count == 0) {
            return;
        }
        Spell_Effect[] confirmedEffects = _loadedSpellEffects.ToArray();
        SpellModifier[] confirmedModifiers = _loadedSpellModifiers.ToArray();
        Spell newSpell = new Spell(_loadedCastingMethod, confirmedEffects, confirmedModifiers);
        // give spell to user
    }
}
