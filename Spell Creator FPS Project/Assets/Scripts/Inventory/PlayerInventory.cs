using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void RunicInventoryDataUpdated(IReadOnlyDictionary<string, int> updatedInventory);
public delegate void SpellInventoryDataUpdated(IReadOnlyList<StorableSpell> updatedSpells);
public delegate void LoadoutDataUpdated(StorableSpell[] Loadout);

public interface IRunicInventory {

    IReadOnlyDictionary<string, int> StoredRunes { get; }

    void AddItem(string id, int count);
    void AddItem(IInventoryStorable storable);
    bool RemoveItem(string id, int count);
    bool HasItem(string id);
    bool HasItemQuantity(string id, int count);
    void SoftRefresh();
    IReadOnlyList<KeyValuePair<string, int>> RetrieveAllItems();
    IReadOnlyList<KeyValuePair<string, int>> RetrieveAllItems(InventoryItemType[] filter);

    event RunicInventoryDataUpdated OnRunicInventoryDataUpdated;
}

public interface ISpellInventory {

    IReadOnlyList<StorableSpell> StoredSpells { get; }
    StorableSpell[] CurrentLoadout { get; }

    void AddSpell(StorableSpell spell);
    void RemoveSpell(StorableSpell spell);
    bool HasSpellByInstanceId(string instanceId);
    StorableSpell GetSpellByInstanceId(string instanceId);
    void SetSpellInLoadout(string spellInstanceId, int index);
    void RemoveSpellInLoadout(int index);
    void SoftRefresh();

    event SpellInventoryDataUpdated OnSpellInventoryDataUpdated;
    event LoadoutDataUpdated OnLoadoutDataUpdated;
}

public class PlayerInventory : MonoBehaviour, IRunicInventory, ISpellInventory{

    public static IRunicInventory RunicInventory { get; private set; }
    public static ISpellInventory SpellInventory { get; private set; }

    private Dictionary<string, int> _storedRunes = new Dictionary<string, int>();
    public IReadOnlyDictionary<string, int> StoredRunes => _storedRunes;
    private List<StorableSpell> _storableSpells = new List<StorableSpell>();
    public IReadOnlyList<StorableSpell> StoredSpells => _storableSpells;
    public StorableSpell[] CurrentLoadout { get; private set; } = new StorableSpell[GameplayValues.Magic.PlayerLoadoutMaxSize];

    public event RunicInventoryDataUpdated OnRunicInventoryDataUpdated;
    public event SpellInventoryDataUpdated OnSpellInventoryDataUpdated;
    public event LoadoutDataUpdated OnLoadoutDataUpdated;

    private void Awake() {
        RunicInventory = this;
        SpellInventory = this;
        InitializeRuneInventory();
        InitializeSpellInventory();
        DontDestroyOnLoad(gameObject);
    }

    private void InitializeRuneInventory() {
        _storedRunes.Clear();
        List<StorableInventoryRune> runeInventory = GameManager.Instance.GetSavedRuneInventory();
        for(int i = 0; i < runeInventory.Count; i++) {
            _storedRunes.Add(runeInventory[i].Id, runeInventory[i].Count);
        }
    }

    private void InitializeSpellInventory() {
        _storableSpells = GameManager.Instance.GetSavedSpellsInventory();
        CurrentLoadout = GameManager.Instance.GetSavedLoadout();
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
        Debug.Log($"Player Inventory added: {id} by {count}!");
    }

    public void AddItem(IInventoryStorable storable) {

    }

    public bool RemoveItem(string id, int count) {
        if (!_storedRunes.ContainsKey(id)) {
            return false;
        }
        _storedRunes[id] -= count;
        if(_storedRunes[id] <= 0) {
            _storedRunes.Remove(id);
        }
        Debug.Log($"Removing {id}...");
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
        if(filter == null || filter.Length == 0) {
            return RetrieveAllItems();
        }
        List<KeyValuePair<string, int>> items = new List<KeyValuePair<string, int>>();
        foreach (KeyValuePair<string, int> pair in _storedRunes) {
            IInventoryStorable inventoryStorable = InventoryRegistry.Instance.GetItemById(pair.Key);
            if(ArrayHelper.Contains(filter, inventoryStorable.ItemType)) {
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

    #region Spells Inventory Methods
    public void AddSpell(StorableSpell spell) {
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
        int index = _storableSpells.FindIndex(x => x.InstanceId.Equals(instanceId));
        if(index == -1) {
            Debug.LogWarning($"[{nameof(PlayerInventory)}] Spell with instance id {instanceId} not found!");
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
        if(spell == null) {
            return;
        }
        if(index > CurrentLoadout.Length || index < 0) {
            Debug.LogError($"[{nameof(PlayerInventory)}] Index {index} out of bounds");
            return;
        }
        CurrentLoadout[index] = spell;
        OnLoadoutDataUpdated?.Invoke(CurrentLoadout);
    }

    private void RemoveSpellFromLoadout(int index) {
        if (index > CurrentLoadout.Length || index < 0) {
            Debug.LogError($"[{nameof(PlayerInventory)}] Index {index} out of bounds");
            return;
        }
        for(int i = index; i < CurrentLoadout.Length; i++) {
            if(i == CurrentLoadout.Length - 1) { // reached end of loadout
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
