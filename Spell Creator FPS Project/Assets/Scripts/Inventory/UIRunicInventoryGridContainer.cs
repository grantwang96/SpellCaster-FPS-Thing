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

    public delegate void InventoryUpdatedEvent();
    public event InventoryUpdatedEvent OnInventoryUpdated;

    protected override void Awake() {
        Inventory = GameManager.GameManagerInstance?.CurrentRunicInventory;
    }

    public override void Initialize(UIPanelInitData initData) {
        base.Initialize(initData);
        if(Inventory == null) { Inventory = GameManager.GameManagerInstance?.CurrentRunicInventory; }
        Inventory.OnRunicInventoryDataUpdated += OnItemsUpdated;
        OnItemsUpdated(Inventory.StoredRunes);
    }
    
    private void OnDisable() {
        if(Inventory != null) {
            Inventory.OnRunicInventoryDataUpdated -= OnItemsUpdated;
        }
    }

    private void OnItemsUpdated(IReadOnlyDictionary<string, int> updatedInventory) {
        _items.Clear();
        foreach(KeyValuePair<string, int> pair in updatedInventory) {
            _items.Add(pair);
        }
        UpdateViewCells();
        OnInventoryUpdated?.Invoke();
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
