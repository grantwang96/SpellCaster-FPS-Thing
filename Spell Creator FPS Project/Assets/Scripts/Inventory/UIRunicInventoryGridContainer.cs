using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIRunicInventoryGridContainer : UIInventoryViewGridContainer {

    [SerializeField] private RectTransform _content;
    [SerializeField] private InventoryViewCell _inventoryViewCellPrefab;

    [SerializeField] private InventoryItemType[] _filter;
    private List<KeyValuePair<string, int>> _items = new List<KeyValuePair<string, int>>();
    private IRunicInventory Inventory;

    protected override void Awake() {
        Inventory = PlayerInventory.RunicInventory;
    }

    public override void Initialize(UIPanelInitData initData) {
        Debug.Log("Initializing Runic Inventory...");
        base.Initialize(initData);
        if(Inventory == null) { Inventory = PlayerInventory.RunicInventory; }
        Inventory.OnRunicInventoryDataUpdated += OnItemsUpdated;
        OnItemsUpdated(Inventory.StoredRunes);
    }
    
    private void OnDisable() {
        if(Inventory != null) {
            Inventory.OnRunicInventoryDataUpdated -= OnItemsUpdated;
        }
        _mainInventoryGrid.OnSelectPressed -= OnSelectPressed;
    }

    private void OnItemsUpdated(IReadOnlyDictionary<string, int> updatedInventory) {
        Debug.Log("Updating inventory view items...");
        _items.Clear();
        foreach(KeyValuePair<string, int> pair in updatedInventory) {
            Debug.Log($"{pair.Key}, {pair.Value}");
            _items.Add(pair);
        }
        UpdateViewCells();
    }

    protected override void SetGridInteractableItem(int x, int y) {
        int startingIndex = _currentPage * _totalPages;
        int columnMod = _columnSize * x;
        int itemIndex = columnMod + y;
        InventoryViewCellData initData = new InventoryViewCellData(x, y) {
            Id = GameplayValues.UI.EmptyInventoryItemId
        };
        if (itemIndex + startingIndex >= _items.Count) {
            _mainInventoryGrid.SetInteractableItem(x, y, initData);
            return;
        }
        KeyValuePair<string, int> currentItem = _items[itemIndex + startingIndex];
        initData.Id = currentItem.Key;
        initData.itemCount = currentItem.Value;
        _mainInventoryGrid.SetInteractableItem(x, y, initData);
    }

    public void AddItem(string id, int count) {
        Inventory.AddItem(id, count);
    }

    public void RemoveItem(string id, int count) {
        Inventory.RemoveItem(id, count);
    }
}
