using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : UIPanel {

    [SerializeField] private RectTransform _content;
    [SerializeField] private InventoryViewCell _inventoryViewCellPrefab;
    private List<InventoryViewCell> viewCells = new List<InventoryViewCell>();

    [SerializeField] private int _rowSize;
    [SerializeField] private int _columnSize;
    [SerializeField] private int _currentPage = 0;
    [SerializeField] private int _totalPages = 1;

    private List<KeyValuePair<string, int>> _items = new List<KeyValuePair<string, int>>();

    private IInventory _inventory;

    public override void Initialize(UIPanelInitData initData) {
        InventoryPanelInitData inventoryInit = initData as InventoryPanelInitData;
        if(inventoryInit != null) {
            Initialize(inventoryInit.Inventory);
        }
    }

    public void Initialize(IInventory inventory) {
        Debug.Log("Initializing inventory view...");
        _inventory = inventory;
        _inventory.OnInventoryDataUpdated += OnItemsUpdated;
        LoadAllItems();
        OnItemsUpdated();
    }

    private void OnDisable() {
        if(_inventory != null) {
            _inventory.OnInventoryDataUpdated -= OnItemsUpdated;
        }
    }

    private void LoadAllItems() {
        if(_inventory == null) {
            Debug.Log("Inventory is null!");
            return;
        }
        _inventoryViewCellPrefab.gameObject.SetActive(true);
        int _itemsPerPage = _rowSize * _columnSize;
        for(int i = 0; i < _itemsPerPage; i++) {
            InventoryViewCell newCell = Instantiate(_inventoryViewCellPrefab, _content);
            viewCells.Add(newCell);
        }
        _items = _inventory.RetrieveAllItems();
        _totalPages = Mathf.CeilToInt(_items.Count / _itemsPerPage);
        _currentPage = 0;
        _inventoryViewCellPrefab.gameObject.SetActive(false);
    }

    private void OnItemsUpdated() {
        Debug.Log("Updating inventory view items...");
        int startingIndex = _currentPage * _totalPages;
        int _itemsPerPage = _rowSize * _columnSize;
        for (int i = 0; i < _itemsPerPage; i++) {
            if (i >= _items.Count) {
                viewCells[i].Initialize("NONE");
                continue;
            }
            KeyValuePair<string, int> currentItem = _items[i + startingIndex];
            viewCells[i].Initialize(currentItem.Key, currentItem.Value);
        }
    }
}
