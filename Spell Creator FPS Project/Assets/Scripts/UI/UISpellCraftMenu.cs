using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISpellCraftMenu : UISubPanelParent {

    [SerializeField] private bool _isSpellCraftMode;

    [SerializeField] private Button _dropdownMenuButtonPrefab;
    [SerializeField] private RectTransform _dropdownMenuContent;

    [SerializeField] private InventoryView _inventoryView;
    [SerializeField] private UISpellStagingArea _spellStagingArea;

    public ISpellCraftManager SpellCraftManager => _spellCraftManager;
    private SpellCraftManager _spellCraftManager;

    public override void Initialize(UIPanelInitData initData) {
        _spellCraftManager = new SpellCraftManager();
        InventoryPanelInitData inventoryInit = initData as InventoryPanelInitData;
        if(inventoryInit != null) {
            _inventoryView.Initialize(inventoryInit.Inventory);
        }
        _inventoryView.ActiveSubPanel = true;
        _inventoryView.OnInventoryItemSelected += OnInventoryItemSelected;
        _spellStagingArea.OnSpellSlotSelected += OnStagingAreaItemSelected;
        _spellStagingArea.OnCraftSpellPressed += GenerateSpell;
        _spellStagingArea.Initialize();

        // add on hover events here(maybe limit if we're on PC or not)
    }

    private void OnDisable() {
        _inventoryView.OnInventoryItemSelected -= OnInventoryItemSelected;
    }

    protected override void Update() {
        base.Update();
    }

    protected override void CloseUIPanel() {
        // return any staged runes to the player
        if(_spellCraftManager.LoadedCastingMethod != null) {
            _inventoryView.Inventory.AddItem(_spellCraftManager.LoadedCastingMethod.Id, 1);
        }
        foreach(Spell_Effect spellEffect in _spellCraftManager.LoadedSpellEffects) {
            _inventoryView.Inventory.AddItem(spellEffect.Id, 1);
        }
        foreach(SpellModifier spellModifier in _spellCraftManager.LoadedSpellModifiers) {
            _inventoryView.Inventory.AddItem(spellModifier.Id, 1);
        }

        // close the panel
        base.CloseUIPanel();
    }

    public override void ChangePanel(IntVector3 dir) {
        switch (dir.x) {
            case 1:
                _inventoryView.ActiveSubPanel = true;
                _spellStagingArea.ActiveSubPanel = false;
                break;
            case -1:
                _inventoryView.ActiveSubPanel = false;
                _spellStagingArea.ActiveSubPanel = true;
                break;
        }
        InvokeSubPanelChanged();
    }

    private void OnInventoryItemSelected() {
        IInventoryStorable inventoryStorable = InventoryRegistry.Instance.GetItemById(_inventoryView.HighlightedItemId);
        Spell_CastingMethod castingMethod = inventoryStorable as Spell_CastingMethod;
        if(castingMethod != null) {
            if(_spellCraftManager.LoadedCastingMethod != null) {
                _inventoryView.Inventory.AddItem(_spellCraftManager.LoadedCastingMethod.Id, 1);
            }
            _inventoryView.Inventory.RemoveItem(_inventoryView.HighlightedItemId, 1);
            _spellCraftManager.SetCastingMethod(castingMethod);
            _spellStagingArea.SetUICastingMethod(castingMethod);
            return;
        }
        Spell_Effect spellEffect = inventoryStorable as Spell_Effect;
        if(spellEffect != null) {
            if (_spellCraftManager.LoadedSpellEffects.Contains(spellEffect)) {
                // do failed effect here
                Debug.Log($"Already contains component {inventoryStorable.Id}");
                return;
            }
            _inventoryView.Inventory.RemoveItem(_inventoryView.HighlightedItemId, 1);
            _spellCraftManager.AddSpellEffect(spellEffect);
            _spellStagingArea.AddUISpellEffect(spellEffect);
            return;
        }
        SpellModifier spellModifier = inventoryStorable as SpellModifier;
        if(spellModifier != null) {
            if (_spellCraftManager.LoadedSpellModifiers.Contains(spellModifier)) {
                // do failed effect here
                Debug.Log($"Already contains component {inventoryStorable.Id}");
                return;
            }
            _inventoryView.Inventory.RemoveItem(_inventoryView.HighlightedItemId, 1);
            _spellCraftManager.AddSpellModifier(spellModifier);
            _spellStagingArea.AddUISpellModifier(spellModifier);
        }
    }

    private void OnStagingAreaItemSelected(string itemId) {
        IInventoryStorable inventoryStorable = InventoryRegistry.Instance.GetItemById(itemId);
        if(inventoryStorable == null) {
            Debug.LogError("Spell component slot contains an invalid inventory itemId!");
            return;
        }
        _inventoryView.Inventory.AddItem(itemId, 1);
        _spellCraftManager.RemoveComponentFromSpell(itemId);
        _spellStagingArea.RemoveHighlightedSpellSlot();
    }

    private void GenerateSpell() {
        if(_spellCraftManager.LoadedCastingMethod == null) {
            return;
        }
        if(_spellCraftManager.LoadedSpellEffects.Count == 0) {
            return;
        }
        if(_spellCraftManager.LoadedSpellModifiers.Count == 0) {
            return;
        }
    }
}
