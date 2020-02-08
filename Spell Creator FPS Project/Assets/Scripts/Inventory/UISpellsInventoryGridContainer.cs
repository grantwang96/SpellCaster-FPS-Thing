using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISpellsInventoryGridContainer : UIInventoryViewGridContainer {

    [SerializeField] private RectTransform _content;
    [SerializeField] private SpellViewCell _spellViewCellPrefab;

    [SerializeField] private Button _leftPageTurnBtn;
    [SerializeField] private Button _rightPageTurnBtn;
    [SerializeField] private Text _pageDisplay;

    [SerializeField] private int _spellsPerPage;

    private ISpellInventory SpellsInventory;
    private List<StorableSpell> _storedSpells = new List<StorableSpell>();
    
    public bool DisplayLoadoutSpells { get; private set; }
    
    public override void Initialize(UIPanelInitData initData) {
        base.Initialize(initData);
        SpellsInventory = GameManager.GameManagerInstance?.CurrentSpellInventory;
        SpellsInventory.OnSpellInventoryDataUpdated += OnInventoryUpdated;
        OnInventoryUpdated(SpellsInventory.StoredSpells);
    }

    private void OnDisable() {
        if (SpellsInventory != null) {
            SpellsInventory.OnSpellInventoryDataUpdated -= OnInventoryUpdated;
        }
    }

    private void OnEnable() {
        if (SpellsInventory != null) {
            SpellsInventory.OnSpellInventoryDataUpdated += OnInventoryUpdated;
        }
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
        _mainInventoryGrid.OnHighlighted += OnItemHighlighted;
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
