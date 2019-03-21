using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void InventoryDataUpdated();

public interface IRunicInventory {

    void AddItem(string id, int count);
    void AddItem(IInventoryStorable storable);
    bool RemoveItem(string id, int count);
    bool HasItem(string id);
    bool HasItemQuantity(string id, int count);
    IReadOnlyList<KeyValuePair<string, int>> RetrieveAllItems();
    IReadOnlyList<KeyValuePair<string, int>> RetrieveAllItems(InventoryItemType[] filter);

    event InventoryDataUpdated OnRunicInventoryDataUpdated;
}

public interface ISpellInventory {

    void AddSpell(StorableSpell spell);
    void RemoveSpell(StorableSpell spell);
    IReadOnlyList<StorableSpell> RetrieveAllSpells();

    event InventoryDataUpdated OnSpellInventoryDataUpdated;
}

public class PlayerInventory : MonoBehaviour, IRunicInventory, ISpellInventory{

    public static IRunicInventory RunicInventory;
    public static ISpellInventory SpellInventory;

    private Dictionary<string, int> _runicInventory = new Dictionary<string, int>();
    private List<StorableSpell> _storableSpells = new List<StorableSpell>();

    public event InventoryDataUpdated OnRunicInventoryDataUpdated;
    public event InventoryDataUpdated OnSpellInventoryDataUpdated;

    private void Awake() {
        RunicInventory = this;
        SpellInventory = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        InitializeRuneInventory();
        InitializeSpellInventory();
    }

    private void InitializeRuneInventory() {
        _runicInventory.Clear();
        List<InventoryRune> runeInventory = GameManager.Instance.GetRuneInventory();
        for(int i = 0; i < runeInventory.Count; i++) {
            _runicInventory.Add(runeInventory[i].Id, runeInventory[i].Count);
        }
    }

    private void InitializeSpellInventory() {
        _storableSpells = GameManager.Instance.GetSpellsInventory();
    }

    #region Runes Inventory Methods
    public void AddItem(string id, int count) {
        if (_runicInventory.ContainsKey(id)) {
            _runicInventory[id] += count;
            OnRunicInventoryDataUpdated?.Invoke();
            return;
        }
        _runicInventory.Add(id, count);
        OnRunicInventoryDataUpdated?.Invoke();
        Debug.Log($"Player Inventory added: {id} by {count}!");
        Debug.Log($"New Quantity for {id} is {_runicInventory[id]}");
    }

    public void AddItem(IInventoryStorable storable) {

    }

    public bool RemoveItem(string id, int count) {
        if (!_runicInventory.ContainsKey(id)) {
            return false;
        }
        _runicInventory[id] -= count;
        if(_runicInventory[id] <= 0) {
            _runicInventory.Remove(id);
        }
        Debug.Log($"Removing {id}...");
        OnRunicInventoryDataUpdated?.Invoke();
        return true;
    }

    public bool HasItem(string id) {
        return _runicInventory.ContainsKey(id);
    }

    public bool HasItemQuantity(string id, int count) {
        return _runicInventory.ContainsKey(id) && _runicInventory[id] >= count;
    }

    public IReadOnlyList<KeyValuePair<string, int>> RetrieveAllItems() {
        List<KeyValuePair<string, int>> items = new List<KeyValuePair<string, int>>();
        foreach (KeyValuePair<string, int> pair in _runicInventory) {
            items.Add(pair);
        }
        return items;
    }

    public IReadOnlyList<KeyValuePair<string, int>> RetrieveAllItems(InventoryItemType[] filter) {
        if(filter == null || filter.Length == 0) {
            return RetrieveAllItems();
        }
        List<KeyValuePair<string, int>> items = new List<KeyValuePair<string, int>>();
        foreach (KeyValuePair<string, int> pair in _runicInventory) {
            IInventoryStorable inventoryStorable = InventoryRegistry.Instance.GetItemById(pair.Key);
            if(ArrayHelper.Contains(filter, inventoryStorable.ItemType)) {
                items.Add(pair);
            }
        }
        return items;
    }
    #endregion

    #region Spells Inventory Methods
    public void AddSpell(StorableSpell spell) {
        _storableSpells.Add(spell);
    }

    public void RemoveSpell(StorableSpell spell) {
        _storableSpells.Remove(spell);
    }

    public IReadOnlyList<StorableSpell> RetrieveAllSpells() {
        return _storableSpells;
    }
    #endregion
}
