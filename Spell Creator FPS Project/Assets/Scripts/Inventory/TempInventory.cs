﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// temporary inventory created during certain game modes
public class TempInventory : IRunicInventory, ISpellInventory
{
    private Dictionary<string, int> _storedRunes = new Dictionary<string, int>();
    public IReadOnlyDictionary<string, int> StoredRunes => _storedRunes;
    private List<StorableSpell> _storableSpells = new List<StorableSpell>();
    public IReadOnlyList<StorableSpell> StoredSpells => _storableSpells;
    public StorableSpell[] CurrentLoadout { get; private set; } = new StorableSpell[GameplayValues.Magic.PlayerLoadoutMaxSize];

    public event RunicInventoryDataUpdated OnRunicInventoryDataUpdated;
    public event SpellInventoryDataUpdated OnSpellInventoryDataUpdated;
    public event LoadoutDataUpdated OnLoadoutDataUpdated;

    public TempInventory(bool usePersistedLoadout, IRunicInventory baseRunicInventory = null, ISpellInventory baseSpellInventory = null) {
        // start the inventory fresh
        _storableSpells = new List<StorableSpell>();
        _storedRunes = new Dictionary<string, int>();
        if (usePersistedLoadout) {
            // copy base inventory items (if any)
            if(baseRunicInventory != null) {
                IReadOnlyList<KeyValuePair<string, int>> allRunes = baseRunicInventory.RetrieveAllItems();
                for(int i = 0; i < allRunes.Count; i++) {
                    AddItem(allRunes[i].Key, allRunes[i].Value);
                }
            }
            if(baseSpellInventory != null) {
                IReadOnlyList<StorableSpell> allSpells = baseSpellInventory.StoredSpells;
                for(int i = 0; i < allSpells.Count; i++) {
                    AddSpell(allSpells[i]);
                }
            }
            CurrentLoadout = SaveManager.Instance.GetSavedLoadout();
            for (int i = 0; i < CurrentLoadout.Length; i++) {
                if(CurrentLoadout[i] == null) {
                    continue;
                }
                AddSpell(CurrentLoadout[i]);
            }
        }
    }

    #region Runes Inventory Methods
    public void AddItem(string id, int count) {
        if (_storedRunes.ContainsKey(id)) {
            _storedRunes[id] += count;
            OnRunicInventoryDataUpdated?.Invoke(StoredRunes);
            return;
        }
        _storedRunes.Add(id, count);
        OnRunicInventoryDataUpdated?.Invoke(StoredRunes);
    }

    public void AddItem(IInventoryStorable storable) {

    }

    public bool RemoveItem(string id, int count) {
        if (!_storedRunes.ContainsKey(id)) {
            return false;
        }
        _storedRunes[id] -= count;
        if (_storedRunes[id] <= 0) {
            _storedRunes.Remove(id);
        }
        OnRunicInventoryDataUpdated?.Invoke(StoredRunes);
        return true;
    }

    public bool HasItem(string id) {
        return _storedRunes.ContainsKey(id);
    }

    public bool HasItemQuantity(string id, int count) {
        return _storedRunes.ContainsKey(id) && _storedRunes[id] >= count;
    }

    public IReadOnlyList<KeyValuePair<string, int>> RetrieveAllItems() {
        List<KeyValuePair<string, int>> items = new List<KeyValuePair<string, int>>();
        foreach (KeyValuePair<string, int> pair in _storedRunes) {
            items.Add(pair);
        }
        return items;
    }

    public IReadOnlyList<KeyValuePair<string, int>> RetrieveAllItems(InventoryItemType[] filter) {
        if (filter == null || filter.Length == 0) {
            return RetrieveAllItems();
        }
        List<KeyValuePair<string, int>> items = new List<KeyValuePair<string, int>>();
        foreach (KeyValuePair<string, int> pair in _storedRunes) {
            IInventoryStorable inventoryStorable = InventoryRegistry.Instance.GetItemById(pair.Key);
            if (ArrayHelper.Contains(filter, inventoryStorable.ItemType)) {
                items.Add(pair);
            }
        }
        return items;
    }
    #endregion

    public void SoftRefresh() {
        OnRunicInventoryDataUpdated?.Invoke(StoredRunes);
        OnSpellInventoryDataUpdated?.Invoke(StoredSpells);
        OnLoadoutDataUpdated?.Invoke(CurrentLoadout);
    }

    void IRunicInventory.ClearAll() {
        _storedRunes.Clear();
        OnRunicInventoryDataUpdated?.Invoke(StoredRunes);
    }

    void ISpellInventory.ClearAll() {
        _storableSpells.Clear();
        CurrentLoadout = new StorableSpell[GameplayValues.Magic.PlayerLoadoutMaxSize];
        OnSpellInventoryDataUpdated?.Invoke(StoredSpells);
        OnLoadoutDataUpdated?.Invoke(CurrentLoadout);
    }

    #region Spells Inventory Methods
    public void AddSpell(StorableSpell spell) {
        if (_storableSpells.Contains(spell)) {
            return;
        }
        _storableSpells.Add(spell);
        OnSpellInventoryDataUpdated?.Invoke(StoredSpells);
    }

    public void RemoveSpell(StorableSpell spell) {
        _storableSpells.Remove(spell);
        OnSpellInventoryDataUpdated?.Invoke(StoredSpells);
    }

    public bool HasSpellByInstanceId(string spellInstanceId) {
        int index = _storableSpells.FindIndex(x => x.InstanceId.Equals(spellInstanceId));
        return index != -1;
    }

    public StorableSpell GetSpellByInstanceId(string instanceId) {
        int index = -1;
        for(int i = 0; i < _storableSpells.Count; i++) {
            if (_storableSpells[i].InstanceId == instanceId) {
                index = i;
                break;
            }
        }
        if (index == -1) {
            Debug.LogWarning($"[{nameof(PersistedInventory)}] Spell with instance id {instanceId} not found!");
            return null;
        }
        return _storableSpells[index];
    }

    public void SetSpellInLoadout(string spellInstanceId, int index) {
        if (string.IsNullOrEmpty(spellInstanceId)) {
            RemoveSpellFromLoadout(index);
            OnLoadoutDataUpdated?.Invoke(CurrentLoadout);
            return;
        }
        StorableSpell spell = GetSpellByInstanceId(spellInstanceId);
        if (spell == null) {
            return;
        }
        if (index > CurrentLoadout.Length || index < 0) {
            Debug.LogError($"[{nameof(PersistedInventory)}] Index {index} out of bounds");
            return;
        }
        CurrentLoadout[index] = spell;
        OnLoadoutDataUpdated?.Invoke(CurrentLoadout);
    }

    private void RemoveSpellFromLoadout(int index) {
        if (index > CurrentLoadout.Length || index < 0) {
            Debug.LogError($"[{nameof(PersistedInventory)}] Index {index} out of bounds");
            return;
        }
        for (int i = index; i < CurrentLoadout.Length; i++) {
            if (i == CurrentLoadout.Length - 1) { // reached end of loadout
                CurrentLoadout[i] = null;
                break;
            }
            CurrentLoadout[i] = CurrentLoadout[i + 1];
        }
    }

    public void RemoveSpellInLoadout(int index) {
        CurrentLoadout[index] = null;
        OnLoadoutDataUpdated?.Invoke(CurrentLoadout);
    }
    #endregion
}
