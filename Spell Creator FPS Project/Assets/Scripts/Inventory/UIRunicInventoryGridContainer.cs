using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIRunicInventoryGridContainer : UISubPanel, IUIViewGridParent {

    [SerializeField] private RectTransform _content;
    [SerializeField] private InventoryViewCell _inventoryViewCellPrefab;

    [SerializeField] private UIViewGrid _inventoryGrid;

    [SerializeField] private InventoryItemType[] _filter;
    private List<KeyValuePair<string, int>> _items = new List<KeyValuePair<string, int>>();
    private IRunicInventory Inventory;
    private bool _isBuilt = false;

    [SerializeField] private int _rowSize;
    [SerializeField] private int _columnSize;
    [SerializeField] private int _currentPage = 0;
    [SerializeField] private int _totalPages = 1;

    public string HighlightedItemId { get; protected set; }

    public delegate void InventoryItemSelected();
    public event InventoryItemSelected OnInventoryItemSelected;
    public event UpdateActiveGrid OnUpdateActiveGrid;

    private void Awake() {
        Inventory = PlayerInventory.RunicInventory;
    }

    public override void Initialize(UIPanelInitData initData) {
        base.Initialize(initData);
        if(Inventory == null) { Inventory = PlayerInventory.RunicInventory; }
        Inventory.OnRunicInventoryDataUpdated += OnItemsUpdated;
        GenerateViewCells();
        OnItemsUpdated(Inventory.StoredRunes);
    }

    private void Initialize(IRunicInventory inventory) {
        Debug.Log("Initializing inventory view...");
        Inventory = inventory;
    }

    private void OnDisable() {
        if(Inventory != null) {
            Inventory.OnRunicInventoryDataUpdated -= OnItemsUpdated;
        }
        _inventoryGrid.OnSelectPressed -= OnSelectPressed;
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
    }

    private void OnItemsUpdated(IReadOnlyDictionary<string, int> updatedInventory) {
        Debug.Log("Updating inventory view items...");
        _items.Clear();
        foreach(KeyValuePair<string, int> pair in updatedInventory) {
            Debug.Log(pair.Key);
            _items.Add(pair);
        }
        UpdateViewCells();
    }

    private void UpdateViewCells() {
        int itemsPerPage = _rowSize * _columnSize;
        _totalPages = Mathf.CeilToInt(_items.Count / itemsPerPage);
        // _currentPage = 0;
        int startingIndex = _currentPage * _totalPages;
        for (int i = 0; i < _rowSize; i++) {
            int columnMod = _columnSize * i;
            for (int j = 0; j < _columnSize; j++) {
                int itemIndex = columnMod + j;
                InventoryViewCellData initData = new InventoryViewCellData(i, j) {
                    Id = GameplayValues.UI.EmptyInventoryItemId
                };
                if (itemIndex + startingIndex >= _items.Count) {
                    Debug.Log($"Index {itemIndex + startingIndex} greater than item count {_items.Count}");
                    _inventoryGrid.SetInteractableItem(i, j, initData);
                    continue;
                }
                Debug.Log("Setting value...");
                KeyValuePair<string, int> currentItem = _items[itemIndex + startingIndex];
                initData.Id = currentItem.Key;
                initData.itemCount = currentItem.Value;
                _inventoryGrid.SetInteractableItem(i, j, initData);
            }
        }
    }

    public void AddItem(string id, int count) {
        Inventory.AddItem(id, count);
    }

    public void RemoveItem(string id, int count) {
        Inventory.RemoveItem(id, count);
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
