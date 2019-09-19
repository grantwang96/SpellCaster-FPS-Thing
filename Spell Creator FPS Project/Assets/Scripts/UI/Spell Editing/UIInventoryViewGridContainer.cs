using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class UIInventoryViewGridContainer : UISubPanel, IUIViewGridParent {
    
    [SerializeField] protected UIViewGrid _mainInventoryGrid;
    protected bool _isBuilt = false;

    [SerializeField] protected int _rowSize;
    [SerializeField] protected int _columnSize;
    [SerializeField] protected int _currentPage = 0;
    [SerializeField] protected int _totalPages = 1;

    public string HighlightedItemId { get; protected set; }

    public delegate void GridItemEvent();
    public event GridItemEvent OnGridItemHighlighted;
    public event GridItemEvent OnGridItemSelected;
    public event UpdateActiveGrid OnUpdateActiveGrid;

    protected virtual void Awake() {
        // initialize inventory source here
    }

    public override void Initialize(UIPanelInitData initData) {
        base.Initialize(initData);
        GenerateViewCells();
    }

    protected void OnSelectPressed(IUIInteractable interactable) {
        HighlightedItemId = interactable.Id;
        OnGridItemSelected?.Invoke();
    }

    protected void OnItemHighlighted(IUIInteractable interactable) {
        HighlightedItemId = interactable.Id;

        GameEventsManager.TestEventArgs.Broadcast(HighlightedItemId);

        OnGridItemHighlighted?.Invoke();
    }

    protected void GenerateViewCells() {
        int[] rowLengths = new int[_rowSize];
        for (int i = 0; i < rowLengths.Length; i++) {
            rowLengths[i] = _columnSize;
        }
        UIViewGridInitData initData = new UIViewGridInitData() {
            RowLengths = rowLengths,
        };
        _mainInventoryGrid.Initialize(initData);
        _mainInventoryGrid.OnHighlighted += OnItemHighlighted;
        _mainInventoryGrid.OnSelectPressed += OnSelectPressed;
    }

    protected virtual void UpdateViewCells() {
        int itemsPerPage = _rowSize * _columnSize;
        _totalPages = GetTotalPages(itemsPerPage);
        for (int i = 0; i < _rowSize; i++) {
            for (int j = 0; j < _columnSize; j++) {
                SetGridInteractableItem(i, j);
            }
        }
        IUIInteractable interactable = _mainInventoryGrid.GetInteractableAt(_mainInventoryGrid.CurrentItemX, _mainInventoryGrid.CurrentItemY);
        HighlightedItemId = interactable.Id;
    }

    protected virtual int GetTotalPages(int itemsPerPage) {
        return 1;
    }

    protected virtual void SetGridInteractableItem(int x, int y) {

    }

    public override void SetFocus(bool active, bool hardLocked, IntVector3 dir) {
        base.SetFocus(active, hardLocked, dir);
        _mainInventoryGrid.SetActive(IsFocused, hardLocked);
        _mainInventoryGrid.UnhighlightCell(_mainInventoryGrid.CurrentItemX, _mainInventoryGrid.CurrentItemY);
        if (IsFocused) {
            _mainInventoryGrid.SetCurrentAtBound(dir);
            _mainInventoryGrid.UpdateHighlightedViewCell(_mainInventoryGrid.CurrentItemX, _mainInventoryGrid.CurrentItemY);
        }
    }

    public void UpdateActiveGrid(UIViewGrid newGrid) {
        _mainInventoryGrid.SetActive(newGrid == _mainInventoryGrid);
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
        _mainInventoryGrid.SetActive(false);
        _mainInventoryGrid.UnhighlightCell(_mainInventoryGrid.CurrentItemX, _mainInventoryGrid.CurrentItemY);
        _parentPanel.ChangePanel(neighbor, dir);
    }

    public override void OnPointerEnter(PointerEventData eventData) {
        _parentPanel.ChangePanel(this, IntVector3.Zero);
    }

}
