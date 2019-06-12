using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 *
 * SHOULD REALLY CONSIDER BUILDING PARENT CLASS FOR INVENTORY VIEWS. 
 * 
 */
public class UISpellsInventoryGridContainer : UISubPanel, IUIViewGridParent {

    [SerializeField] private RectTransform _content;
    [SerializeField] private SpellViewCell _spellViewCellPrefab;
    [SerializeField] private UIViewGrid _viewGrid;
    [SerializeField] private int _spellsPerPage;
    [SerializeField] private int _columnSize;
    [SerializeField] private int _rowSize;
    [SerializeField] private int _currentPage = 0;
    [SerializeField] private int _totalPages = 1;

    private ISpellInventory SpellsInventory;
    private List<StorableSpell> _storedSpells = new List<StorableSpell>();

    public delegate void SpellSelectedDelegate();
    public event SpellSelectedDelegate OnSpellSelected; // allows varying events to occur when selecting a spell(editing, selling, deleting, etc.)
    public event UpdateActiveGrid OnUpdateActiveGrid;

    public string HighlightedSpellInstanceId { get; private set; }

    private void Awake() {
        Initialize(PlayerInventory.SpellInventory);
    }

    private void Initialize(ISpellInventory inventory) {
        SpellsInventory = inventory;
        SpellsInventory.OnSpellInventoryDataUpdated += OnInventoryUpdated;
        GenerateViewCells();
        OnInventoryUpdated(SpellsInventory.StoredSpells);
    }

    private void GenerateViewGrid() {
        int[] rows = new int[_rowSize];
        for(int i = 0; i < rows.Length; i++) {
            rows[i] = _columnSize;
        }
        UIViewGridInitData viewGridInitData = new UIViewGridInitData() {
            RowLengths = rows
        };
        _viewGrid.Initialize(viewGridInitData);
        _viewGrid.OnSelectPressed += OnSelectPressed;
    }

    public void OutOfBounds(IntVector3 dir) {
        UISubPanel neighbor;
        if (dir == IntVector3.Up) {
            neighbor = _upNeighbor;
        } else if (dir == IntVector3.Right) {
            neighbor = _rightNeighbor;
        } else if (dir == IntVector3.Left) {
            neighbor = _leftNeighbor;
        } else {
            neighbor = _downNeighbor;
        }
        if(neighbor == null) {
            return;
        }
        // set grid to not active
        _viewGrid.Active = false;
        _viewGrid.UnhighlightCell(_viewGrid.CurrentItemX, _viewGrid.CurrentItemY);
        _parentPanel.ChangePanel(neighbor, dir);
    }

    public void UpdateActiveGrid(UIViewGrid newGrid) {
        _viewGrid.Active = newGrid == _viewGrid;
    }

    private void GenerateViewCells() {
        if(SpellsInventory == null) {
            Debug.LogError($"[{nameof(UISpellsInventoryGridContainer)}] Spells Inventory was null!");
            return;
        }

        int[] rowLengths = new int[_rowSize];
        for (int i = 0; i < rowLengths.Length; i++) {
            rowLengths[i] = _columnSize;
        }
        UIViewGridInitData initData = new UIViewGridInitData() {
            RowLengths = rowLengths,
        };
        _viewGrid.Initialize(initData);
        _viewGrid.OnSelectPressed += OnSelectPressed;
    }

    private void OnInventoryUpdated(IReadOnlyList<StorableSpell> updatedSpells) {
        _storedSpells.Clear();
        _storedSpells.AddRange(updatedSpells);
        UpdateView();
    }

    private void OnSelectPressed(IUIInteractable interactable) {
        HighlightedSpellInstanceId = interactable.Id;
        OnInventoryUpdated(SpellsInventory.StoredSpells);
    }

    private void UpdateView() {
        int spellsPerPage = _rowSize * _columnSize;
        _totalPages = Mathf.CeilToInt(_storedSpells.Count / spellsPerPage);
        // _currentPage = 0;
        int startingIndex = _currentPage * _totalPages;
        for (int i = 0; i < _rowSize; i++) {
            int columnMod = _columnSize * i;
            for (int j = 0; j < _columnSize; j++) {
                int itemIndex = columnMod + j;
                SpellViewCellData initData = new SpellViewCellData(i, j);
                if (itemIndex + startingIndex >= _storedSpells.Count) {
                    _viewGrid.SetInteractableItem(i, j, initData);
                    continue;
                }
                StorableSpell currentSpell = _storedSpells[itemIndex + startingIndex];
                initData.Id = currentSpell.InstanceId;
                initData.SetValue(currentSpell);
                _viewGrid.SetInteractableItem(i, j, initData);
            }
        }
    }
}
