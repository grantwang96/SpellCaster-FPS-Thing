using System.Collections.Generic;
using UnityEngine;

public class UIRunicInventoryGridContainer : UIInventoryViewGridContainer {

    [SerializeField] private RectTransform _content;
    [SerializeField] private InventoryViewCell _inventoryViewCellPrefab;

    [SerializeField] private InventoryItemType[] _filter;
    private List<KeyValuePair<string, int>> _items = new List<KeyValuePair<string, int>>();

    public delegate void InventoryUpdatedEvent();
    public event InventoryUpdatedEvent OnInventoryUpdated;

    public override void Initialize(UIPanelInitData initData) {
        base.Initialize(initData);
        if(GameManager.GameManagerInstance?.CurrentRunicInventory == null) {
            CustomLogger.Error(nameof(UIRunicInventoryGridContainer), $"Current {nameof(IRunicInventory)} is unavailable!");
            return;
        }
        GameManager.GameManagerInstance.CurrentRunicInventory.OnRunicInventoryDataUpdated += OnItemsUpdated;
        OnItemsUpdated(GameManager.GameManagerInstance?.CurrentRunicInventory.StoredRunes);
    }
    
    private void OnDisable() {
        if(GameManager.GameManagerInstance?.CurrentRunicInventory != null) {
            GameManager.GameManagerInstance.CurrentRunicInventory.OnRunicInventoryDataUpdated -= OnItemsUpdated;
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
        GameManager.GameManagerInstance?.CurrentRunicInventory.AddItem(id, count);
    }

    public void RemoveItem(string id, int count) {
        GameManager.GameManagerInstance?.CurrentRunicInventory.RemoveItem(id, count);
    }
}
