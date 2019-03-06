using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// NOTE: current cell x and y are reversed here
// i.e. viewCell[_currentCellY][_currentCellX].Highlight()
public class InventoryView : UISubPanel {

    [SerializeField] private RectTransform _content;
    [SerializeField] private InventoryViewCell _inventoryViewCellPrefab;

    private InventoryViewCell[][] viewCells;

    private InventoryItemType[] _filter;
    private List<KeyValuePair<string, int>> _items = new List<KeyValuePair<string, int>>();
    public IInventory Inventory { get; private set; }

    [SerializeField] private int _rowSize;
    [SerializeField] private int _columnSize;
    [SerializeField] private int _currentPage = 0;
    [SerializeField] private int _totalPages = 1;
    [Range(0f, 1f)] [SerializeField] private float _directionHoldThreshold;
    [Range(0f, 1f)] [SerializeField] private float _directionHoldFreq;

    [SerializeField] private int _currentCellX;
    [SerializeField] private int _currentCellY;
    public string HighlightedItemId { get; protected set; }

    public delegate void InventoryItemSelected();
    public event InventoryItemSelected OnInventoryItemSelected;

    private float _horizontal;
    private float _vertical;
    private float _intervalHoldTime;
    private float _holdTime;
    private bool _directionButtonsPressed;

    public override void Initialize(UIPanelInitData initData) {
        InventoryPanelInitData inventoryInit = initData as InventoryPanelInitData;
        if(inventoryInit != null) {
            _filter = inventoryInit.Filter;
            Initialize(inventoryInit.Inventory);
            return;
        }
        Debug.LogError($"{initData.ToString()} is not an InventoryPanelInitData");
    }

    public void Initialize(IInventory inventory) {
        Debug.Log("Initializing inventory view...");
        Inventory = inventory;

        Inventory.OnInventoryDataUpdated += OnItemsUpdated;
        _parentPanel.OnSubPanelChanged += OnSubPanelChanged;

        GenerateViewCells();
        OnItemsUpdated();
        UpdateHighlightedViewCell(0, 0);
    }

    private void OnDisable() {
        if(Inventory != null) {
            Inventory.OnInventoryDataUpdated -= OnItemsUpdated;
        }
        foreach(Transform t in _content) {
            Destroy(t.gameObject);
        }
    }

    protected override void ProcessInputs() {
        DirectionalMovement();
        SelectPressed();
    }

    private void DirectionalMovement() {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");

        // if no buttons are being pressed, reset values and carry on.
        if(_horizontal == 0 && _vertical == 0) {
            _holdTime = 0f;
            _intervalHoldTime = 0f;
            _directionButtonsPressed = false;
            return;
        }

        // if the button is being held
        if (_directionButtonsPressed) {
            if(_holdTime < _directionHoldThreshold) { // check if they're waiting to do continuous movement
                _holdTime += Time.deltaTime;
                return;
            }
            if(_intervalHoldTime < _directionHoldFreq) { // check if they're waiting on interval
                _intervalHoldTime += Time.deltaTime;
                return;
            }
        }
        
        // Process the actual movement;
        _directionButtonsPressed = true;
        _intervalHoldTime = 0f;
        
        int x = _currentCellX + Mathf.RoundToInt(_horizontal);
        int y = _currentCellY - Mathf.RoundToInt(_vertical);
        
        if(x < 0) {
            _parentPanel.ChangePanel(new IntVector3(-1, 0, 0));
            return;
        }
        if(x >= _rowSize) {
            _parentPanel.ChangePanel(new IntVector3(1, 0, 0));
            return;
        }
        if(y < 0) {
            _parentPanel.ChangePanel(new IntVector3(0, -1, 0));
            return;
        }
        if(y >= _columnSize) {
            _parentPanel.ChangePanel(new IntVector3(0, 1, 0));
            return;
        }

        UpdateHighlightedViewCell(x, y);
    }

    private void SelectPressed() {
        if (Input.GetButtonDown("Submit")) {
            InventoryViewCell viewCell = viewCells[_currentCellY][_currentCellX];
            Debug.Log($"View Cell {viewCell.ItemId} selected!");
            OnInventoryItemSelected?.Invoke();
        }
    }

    private void UpdateHighlightedViewCell(int x, int y) {
        viewCells[_currentCellY][_currentCellX].Unhighlight();
        _currentCellX = x;
        _currentCellY = y;
        viewCells[y][x].Highlight();
        HighlightedItemId = viewCells[y][x].ItemId;
    }

    private void GenerateViewCells() {
        if(Inventory == null) {
            Debug.Log("Inventory is null!");
            return;
        }
        _inventoryViewCellPrefab.gameObject.SetActive(true);

        viewCells = new InventoryViewCell[_columnSize][];
        for (int i = 0; i < _columnSize; i++) {
            viewCells[i] = new InventoryViewCell[_rowSize];
            for(int j = 0; j < _rowSize; j++) {
                InventoryViewCell newCell = Instantiate(_inventoryViewCellPrefab, _content);
                viewCells[i][j] = newCell;
            }
        }
        _items = Inventory.RetrieveAllItems();
        _inventoryViewCellPrefab.gameObject.SetActive(false);
    }

    private void OnItemsUpdated() {
        Debug.Log("Updating inventory view items...");
        _items = Inventory.RetrieveAllItems();
        int _itemsPerPage = _rowSize * _columnSize;
        _totalPages = Mathf.CeilToInt(_items.Count / _itemsPerPage);
        _currentPage = 0;
        int startingIndex = _currentPage * _totalPages;
        for(int i = 0; i < _columnSize; i++) {
            int columnMod = _columnSize * i;
            for(int j = 0; j < _rowSize; j++) {
                int itemIndex = columnMod + j;
                InventoryViewCellInitData initData = new InventoryViewCellInitData {
                    inventoryView = this,
                    x = i,
                    y = j,
                    itemId = GameplayValues.EmptyInventoryItemId
                };
                if (itemIndex + startingIndex >= _items.Count) {
                    viewCells[i][j].Initialize(initData);
                    continue;
                }
                KeyValuePair<string, int> currentItem = _items[itemIndex + startingIndex];
                initData.itemId = currentItem.Key;
                initData.itemCount = currentItem.Value;
                viewCells[i][j].Initialize(initData);
            }
        }
        UpdateHighlightedViewCell(_currentCellY, _currentCellX);
    }

    protected override void OnSubPanelChanged() {
        if (!ActiveSubPanel) {
            viewCells[_currentCellY][_currentCellX].Unhighlight();
        } else {
            viewCells[_currentCellY][_currentCellX].Highlight();
        }
    }
}
