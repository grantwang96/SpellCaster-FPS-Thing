using System.Collections.Generic;
using UnityEngine;

public class UIRunicInventoryGridContainer : UIInventoryViewGridContainer {

    [SerializeField] private RectTransform _content;
    [SerializeField] private InventoryViewCell _inventoryViewCellPrefab;

    [SerializeField] private InventoryItemType[] _filter;
    private List<RuneInfo> _items = new List<RuneInfo>();

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
            _items.Add(new RuneInfo(pair.Key, pair.Value));
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
        RuneInfo currentItem = _items[itemIndex + startingIndex];
        initData.Id = currentItem.Id;
        initData.itemCount = currentItem.Count;
        _mainInventoryGrid.SetInteractableItem(x, y, initData);
    }

    public void AddItem(string id, int count) {
        bool hasItem = false;
        for(int i = 0; i < _items.Count; i++) {
            if(_items[i].Id == id) {
                _items[i].Count += count;
                hasItem = true;
                break;
            }
        }
        if (!hasItem) {
            _items.Add(new RuneInfo(id, count));
        }
        UpdateViewCells();
        OnInventoryUpdated?.Invoke();
    }

    public void RemoveItem(string id, int count) {
        RuneInfo info = _items.Find(x => x.Id == id);
        if(info != null) {
            info.Count -= count;
            if(info.Count <= 0) {
                _items.Remove(info);
            }   
        }
        UpdateViewCells();
        OnInventoryUpdated?.Invoke();
    }
}

public class RuneInfo {
    public string Id;
    public int Count;

    public RuneInfo() {

    }

    public RuneInfo(string id, int count) {
        Id = id;
        Count = count;
    }
}
