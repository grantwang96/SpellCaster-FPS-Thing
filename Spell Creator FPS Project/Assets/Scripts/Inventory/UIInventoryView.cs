using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIInventoryView : UISubPanel, IUIViewGridParent {

    [SerializeField] private RectTransform _content;
    [SerializeField] private InventoryViewCell _inventoryViewCellPrefab;

    [SerializeField] private UIViewGrid _inventoryGrid;

    [SerializeField] private InventoryItemType[] _filter;
    private IReadOnlyList<KeyValuePair<string, int>> _items;
    public IRunicInventory Inventory { get; private set; }

    [SerializeField] private int _rowSize;
    [SerializeField] private int _columnSize;
    [SerializeField] private int _currentPage = 0;
    [SerializeField] private int _totalPages = 1;

    public string HighlightedItemId { get; protected set; }

    public delegate void InventoryItemSelected();
    public event InventoryItemSelected OnInventoryItemSelected;
    public event UpdateActiveGrid OnUpdateActiveGrid;
    
    public override void Initialize(UIPanelInitData initData) {
        InventoryPanelInitData inventoryInit = initData as InventoryPanelInitData;
        if(inventoryInit != null) {
            _filter = inventoryInit.Filter;
            Initialize(inventoryInit.Inventory);
            return;
        }
        Debug.LogError($"{initData.ToString()} is not an InventoryPanelInitData");
    }

    public void Initialize(IRunicInventory inventory) {
        Debug.Log("Initializing inventory view...");
        Inventory = inventory;
        Inventory.OnRunicInventoryDataUpdated += OnItemsUpdated;
        GenerateViewCells();
        OnItemsUpdated();
    }

    private void OnDisable() {
        if(Inventory != null) {
            Inventory.OnRunicInventoryDataUpdated -= OnItemsUpdated;
        }
        foreach(Transform t in _content) {
            Destroy(t.gameObject);
        }
    }
    
    private void OnSelectPressed(IUIInteractable interactable) {
        HighlightedItemId = interactable.Id;
        OnInventoryItemSelected?.Invoke();
    }

    private void GenerateViewCells() {
        if(Inventory == null) {
            Debug.Log("Inventory is null!");
            return;
        }

        int[] rowLengths = new int[_rowSize];
        for(int i = 0; i < rowLengths.Length; i++) {
            rowLengths[i] = _columnSize;
        }
        UIViewGridInitData initData = new UIViewGridInitData() {
            RowLengths = rowLengths,
        };
        _inventoryGrid.Initialize(initData);
        _inventoryGrid.OnSelectPressed += OnSelectPressed;

        OnItemsUpdated();
    }

    private void OnItemsUpdated() {
        Debug.Log("Updating inventory view items...");
        _items = Inventory.RetrieveAllItems(_filter);
        int _itemsPerPage = _rowSize * _columnSize;
        _totalPages = Mathf.CeilToInt(_items.Count / _itemsPerPage);
        _currentPage = 0;
        int startingIndex = _currentPage * _totalPages;
        for(int i = 0; i < _rowSize; i++) {
            int columnMod = _columnSize * i;
            for(int j = 0; j < _columnSize; j++) {
                int itemIndex = columnMod + j;
                InventoryViewCellData initData = new InventoryViewCellData(i, j) {
                    itemId = GameplayValues.UI.EmptyInventoryItemId
                };
                if (itemIndex + startingIndex >= _items.Count) {
                    _inventoryGrid.SetInteractableItem(i, j, initData);
                    continue;
                }
                KeyValuePair<string, int> currentItem = _items[itemIndex + startingIndex];
                initData.itemId = currentItem.Key;
                initData.itemCount = currentItem.Value;
                _inventoryGrid.SetInteractableItem(i, j, initData);
            }
        }
    }

    public override void SetActive(bool active, IntVector3 dir) {
        _inventoryGrid.Active = active;
        if (active) {
            _inventoryGrid.SetCurrentAtBound(dir);
            _inventoryGrid.UpdateHighlightedViewCell(_inventoryGrid.CurrentItemX, _inventoryGrid.CurrentItemY);
        } else {
            _inventoryGrid.UnhighlightCell(_inventoryGrid.CurrentItemX, _inventoryGrid.CurrentItemY);
        }
    }

    public void UpdateActiveGrid(UIViewGrid newGrid) {
        _inventoryGrid.Active = newGrid == _inventoryGrid;
    }

    public void OutOfBounds(IntVector3 dir) {
        UISubPanel neighbor;
        if (dir == IntVector3.Up) {
            neighbor = _upNeighbor;
        } else if (dir == IntVector3.Right) {
            neighbor = _rightNeighbor;
        } else if (dir == IntVector3.Down) {
            neighbor = _downNeighbor;
        } else {
            neighbor = _leftNeighbor;
        }
        if (neighbor == null) {
            return;
        }
        _inventoryGrid.Active = false;
        _inventoryGrid.UnhighlightCell(_inventoryGrid.CurrentItemX, _inventoryGrid.CurrentItemY);
        _parentPanel.ChangePanel(neighbor, dir);
    }

    public override void OnPointerEnter(PointerEventData eventData) {
        _parentPanel.ChangePanel(this, IntVector3.Zero);
    }
}
