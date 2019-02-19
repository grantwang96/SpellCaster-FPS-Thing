using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : UISubPanel {

    [SerializeField] private RectTransform _content;
    [SerializeField] private InventoryViewCell _inventoryViewCellPrefab;

    private InventoryViewCell[][] viewCells;

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
            Initialize(inventoryInit.Inventory);
            return;
        }
        Debug.LogError($"{initData.ToString()} is not an InventoryPanelInitData");
    }

    public void Initialize(IInventory inventory) {
        Debug.Log("Initializing inventory view...");
        Inventory = inventory;
        Inventory.OnInventoryDataUpdated += OnItemsUpdated;
        GenerateViewCells();
        OnItemsUpdated();
        UpdateHighlightedViewCell();
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
        if(x < 0) { x = _rowSize - 1; }
        if(x >= _rowSize) { x = 0; }
        if(y < 0) { y = _columnSize - 1; }
        if(y >= _columnSize) { y = 0; }
        _currentCellX = x;
        _currentCellY = y;
        UpdateHighlightedViewCell();
    }

    private void SelectPressed() {
        if (Input.GetButtonDown("Submit")) {
            InventoryViewCell viewCell = viewCells[_currentCellY][_currentCellX];
            Debug.Log($"View Cell {viewCell.ItemId} selected!");
            OnInventoryItemSelected?.Invoke();
        }
    }

    private void UpdateHighlightedViewCell() {
        for(int i = 0; i < _columnSize; i++) {
            for(int j = 0; j < _rowSize; j++) {
                if(j == _currentCellX && i == _currentCellY) {
                    viewCells[i][j].Highlight();
                    HighlightedItemId = viewCells[i][j].ItemId;
                    continue;
                }
                viewCells[i][j].Dehighlight();
            }
        }
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
                if (itemIndex + startingIndex >= _items.Count) {
                    viewCells[i][j].Initialize(this, i, j, GameplayValues.EmptyInventoryItemId);
                    continue;
                }
                KeyValuePair<string, int> currentItem = _items[itemIndex + startingIndex];
                viewCells[i][j].Initialize(this, i, j, currentItem.Key, currentItem.Value);
            }
        }
        UpdateHighlightedViewCell();
    }
}
