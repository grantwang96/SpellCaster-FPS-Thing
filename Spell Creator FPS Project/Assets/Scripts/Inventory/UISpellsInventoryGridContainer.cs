using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 *
 * SHOULD REALLY CONSIDER BUILDING PARENT CLASS FOR INVENTORY VIEWS. 
 * 
 */
public class UISpellsInventoryGridContainer : UIInventoryViewGridContainer {

    [SerializeField] private RectTransform _content;
    [SerializeField] private SpellViewCell _spellViewCellPrefab;

    [SerializeField] private Button _leftPageTurnBtn;
    [SerializeField] private Button _rightPageTurnBtn;
    [SerializeField] private Text _pageDisplay;

    [SerializeField] private int _spellsPerPage;

    private ISpellInventory SpellsInventory;
    private List<StorableSpell> _storedSpells = new List<StorableSpell>();

    public delegate void SpellSelectedDelegate();
    public event SpellSelectedDelegate OnSpellSelected; // allows varying events to occur when selecting a spell(editing, selling, deleting, etc.)
    public delegate void SpellHighlightedDelegate();
    public event SpellHighlightedDelegate OnSpellHighlighted;
    
    public string HighlightedSpellInstanceId { get; private set; }

    protected override void Awake() {
        Initialize(PlayerInventory.SpellInventory);
    }

    private void Initialize(ISpellInventory inventory) {
        SpellsInventory = inventory;
        SpellsInventory.OnSpellInventoryDataUpdated += OnInventoryUpdated;
        GenerateViewCells();
        OnInventoryUpdated(SpellsInventory.StoredSpells);
    }

    public override void Initialize(UIPanelInitData initData) {
        base.Initialize(initData);
    }

    private void GenerateViewGrids() {
        int[] rows = new int[_rowSize];
        for(int i = 0; i < rows.Length; i++) {
            rows[i] = _columnSize;
        }
        UIViewGridInitData spellsGridInitData = new UIViewGridInitData() {
            RowLengths = rows
        };
        _mainInventoryGrid.Initialize(spellsGridInitData);
        _mainInventoryGrid.OnSelectPressed += OnSelectPressed;
    }

    private void OnInventoryUpdated(IReadOnlyList<StorableSpell> updatedSpells) {
        _storedSpells.Clear();
        for(int i = 0; i < updatedSpells.Count; i++) {
            
            _storedSpells.Add(updatedSpells[i]);
        }
        UpdateViewCells();
    }

    protected override int GetTotalPages(int itemsPerPage) {
        return Mathf.CeilToInt(_storedSpells.Count / itemsPerPage);
    }

    protected override void SetGridInteractableItem(int x, int y) {
        int startingIndex = _currentPage * _totalPages;
        int columnMod = _columnSize * x;
        int itemIndex = columnMod + y;
        SpellViewCellData initData = new SpellViewCellData(x, y);
        if (itemIndex + startingIndex >= _storedSpells.Count) {
            _mainInventoryGrid.SetInteractableItem(x, y, initData);
            return;
        }
        StorableSpell currentSpell = _storedSpells[itemIndex + startingIndex];
        initData.Id = currentSpell.InstanceId;
        initData.SetValue(currentSpell);
        _mainInventoryGrid.SetInteractableItem(x, y, initData);
    }
}

public class SpellsInventoryViewInitData : UIPanelInitData {
    public readonly bool ShowLoadoutSpells;

    public SpellsInventoryViewInitData(bool showLoadoutSpells = false) {
        ShowLoadoutSpells = showLoadoutSpells;
    }
}
