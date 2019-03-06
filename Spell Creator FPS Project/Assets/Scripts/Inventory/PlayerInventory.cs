using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void InventoryDataUpdated();

public interface IInventory {

    void AddItem(string id, int count);
    void AddItem(IInventoryStorable storable);
    bool RemoveItem(string id, int count);
    bool HasItem(string id);
    bool HasItemQuantity(string id, int count);
    List<KeyValuePair<string, int>> RetrieveAllItems();
    List<KeyValuePair<string, int>> RetrieveAllItems(InventoryItemType[] filter);

    event InventoryDataUpdated OnInventoryDataUpdated;
}

public class PlayerInventory : MonoBehaviour, IInventory{

    private Dictionary<string, int> _playerInventory = new Dictionary<string, int>(); // the actual data structure

    public event InventoryDataUpdated OnInventoryDataUpdated;

    public void AddItem(string id, int count) {
        if (_playerInventory.ContainsKey(id)) {
            _playerInventory[id] += count;
            OnInventoryDataUpdated?.Invoke();
            return;
        }
        _playerInventory.Add(id, count);
        OnInventoryDataUpdated?.Invoke();
        Debug.Log($"Player Inventory added: {id} by {count}!");
        Debug.Log($"New Quantity for {id} is {_playerInventory[id]}");
    }

    public void AddItem(IInventoryStorable storable) {

    }

    public bool RemoveItem(string id, int count) {
        if (!_playerInventory.ContainsKey(id)) {
            return false;
        }
        _playerInventory[id] -= count;
        if(_playerInventory[id] <= 0) {
            _playerInventory.Remove(id);
        }
        Debug.Log($"Removing {id}...");
        OnInventoryDataUpdated?.Invoke();
        return true;
    }

    public bool HasItem(string id) {
        return _playerInventory.ContainsKey(id);
    }

    public bool HasItemQuantity(string id, int count) {
        return _playerInventory.ContainsKey(id) && _playerInventory[id] >= count;
    }

    public List<KeyValuePair<string, int>> RetrieveAllItems() {
        List<KeyValuePair<string, int>> items = new List<KeyValuePair<string, int>>();
        foreach (KeyValuePair<string, int> pair in _playerInventory) {
            items.Add(pair);
        }
        return items;
    }

    public List<KeyValuePair<string, int>> RetrieveAllItems(InventoryItemType[] filter) {
        if(filter == null || filter.Length == 0) {
            return RetrieveAllItems();
        }
        List<KeyValuePair<string, int>> items = new List<KeyValuePair<string, int>>();
        foreach (KeyValuePair<string, int> pair in _playerInventory) {
            IInventoryStorable inventoryStorable = InventoryRegistry.Instance.GetItemById(pair.Key);
            if(ArrayHelper.Contains(filter, inventoryStorable.ItemType)) {
                items.Add(pair);
            }
        }
        return items;
    }
}
