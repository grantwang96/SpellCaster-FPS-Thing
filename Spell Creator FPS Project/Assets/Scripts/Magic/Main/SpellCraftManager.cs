using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CastingMethodChanged(Spell_CastingMethod castingMethod);
public delegate void SpellEffectChanged(Effect spellEffect);
public delegate void SpellModifierChanged(SpellModifier modifier);

public interface ISpellCraftManager {
    event CastingMethodChanged OnCastingMethodChanged;
    event SpellEffectChanged OnSpellEffectAdded;
    event SpellModifierChanged OnSpellModifierAdded;
    event SpellEffectChanged OnSpellEffectRemoved;
    event SpellModifierChanged OnSpellModifierRemoved;
}

public class SpellCraftManager : ISpellCraftManager {

    [SerializeField] private Spell_CastingMethod _loadedCastingMethod;
    public Spell_CastingMethod LoadedCastingMethod {
        get {
            return _loadedCastingMethod;
        }
    }
    [SerializeField] private List<Effect> _loadedSpellEffects = new List<Effect>();
    public List<Effect> LoadedSpellEffects {
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

    public event CastingMethodChanged OnCastingMethodChanged;
    public event SpellEffectChanged OnSpellEffectAdded;
    public event SpellEffectChanged OnSpellEffectRemoved;
    public event SpellModifierChanged OnSpellModifierAdded;
    public event SpellModifierChanged OnSpellModifierRemoved;

    [SerializeField] private int _loadedSpellManaCost;

    public void SetCastingMethod(Spell_CastingMethod castingMethod) {
        _loadedCastingMethod = castingMethod;
        OnCastingMethodChanged?.Invoke(castingMethod);
    }

    public void AddSpellEffect(Effect spellEffect) {
        if (!_loadedSpellEffects.Contains(spellEffect)) {
            _loadedSpellEffects.Add(spellEffect);
            OnSpellEffectAdded?.Invoke(spellEffect);
            UpdateSpellManaCost();
        }
    }

    public void AddSpellModifier(SpellModifier spellModifier) {
        if (!_loadedSpellModifiers.Contains(spellModifier)) {
            _loadedSpellModifiers.Add(spellModifier);
            OnSpellModifierAdded?.Invoke(spellModifier);
        }
    }

    public void RemoveComponentFromSpell(string itemId) {
        IInventoryStorable storable = InventoryRegistry.Instance.GetItemById(itemId);
        Spell_CastingMethod castingMethod = storable as Spell_CastingMethod;
        if (castingMethod != null) {
            _loadedCastingMethod = null;
            OnCastingMethodChanged?.Invoke(castingMethod);
            return;
        }
        Effect spellEffect = storable as Effect;
        if (spellEffect != null) {
            _loadedSpellEffects.Remove(spellEffect);
            UpdateSpellManaCost();
            OnSpellEffectRemoved?.Invoke(spellEffect);
            return;
        }
        SpellModifier spellModifier = storable as SpellModifier;
        if (spellModifier != null) {
            _loadedSpellModifiers.Remove(spellModifier);
            OnSpellModifierRemoved?.Invoke(spellModifier);
            return;
        }
    }

    private void UpdateSpellManaCost() {
        _loadedSpellManaCost = 0;
        for(int i = 0; i < _loadedSpellEffects.Count; i++) {
            _loadedSpellManaCost += _loadedSpellEffects[i].ManaCost;
        }
    }

    public StorableSpell GenerateSpell() {
        if(_loadedCastingMethod == null || _loadedSpellEffects.Count == 0) {
            return null;
        }
        string[] spellEffectIds = new string[_loadedSpellEffects.Count];
        for(int i = 0; i < _loadedSpellEffects.Count; i++) {
            spellEffectIds[i] = _loadedSpellEffects[i].Id;
        }
        string[] spellModifierIds = new string[_loadedSpellModifiers.Count];
        for(int i = 0; i < _loadedSpellModifiers.Count; i++) {
            spellModifierIds[i] = _loadedSpellModifiers[i].Id;
        }
        StorableSpell newSpell = new StorableSpell(_loadedCastingMethod.Id, spellEffectIds, spellModifierIds);
        return newSpell;
    }

    public void ClearSpellComponents() {
        _loadedCastingMethod = null;
        OnCastingMethodChanged?.Invoke(null);
        for(int i = 0; i < _loadedSpellEffects.Count; i++) {
            OnSpellEffectRemoved?.Invoke(_loadedSpellEffects[i]);
        }
        _loadedSpellEffects.Clear();
        for(int i = 0; i < _loadedSpellModifiers.Count; i++) {
            OnSpellModifierRemoved?.Invoke(_loadedSpellModifiers[i]);
        }
        _loadedSpellModifiers.Clear();
    }
}
