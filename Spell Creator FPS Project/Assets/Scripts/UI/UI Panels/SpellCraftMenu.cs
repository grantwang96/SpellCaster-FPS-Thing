using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class handles crafting a spell for the player
/// </summary>
public class SpellCraftMenu : UISubPanelParent {

    [SerializeField] private string _loadoutPrefabId;
    
    [SerializeField] private Button _dropdownMenuButtonPrefab;
    [SerializeField] private RectTransform _dropdownMenuContent;

    [SerializeField] private UIRunicInventoryGridContainer _runicInventoryView;
    [SerializeField] private UISpellStagingArea _spellStagingArea;
    [SerializeField] private UISpellComponentDescView _spellComponentDescriptionView;

    public override void Initialize(UIPanelInitData initData = null) {
        base.Initialize(initData);
        _spellStagingArea.Initialize(null);
        _runicInventoryView.Initialize(null);
        _runicInventoryView.SetFocus(true, false, IntVector3.Zero);
        _runicInventoryView.SetVisible(true);
        _spellStagingArea.SetFocus(false, false, IntVector3.Zero);
        _spellStagingArea.SetVisible(true);

        // add on hover events here(maybe limit if we're on PC or not)
        SubscribeToSubPanels();
        _spellComponentDescriptionView.UpdateDescription(_runicInventoryView.HighlightedItemId);
    }

    private void SubscribeToSubPanels() {
        _runicInventoryView.OnGridItemHighlighted += OnInventoryItemHighlighted;
        _runicInventoryView.OnGridItemSelected += OnInventoryItemSelected;
        _spellStagingArea.OnSpellSlotHighlighted += OnStagingAreaItemHighlighted;
        _spellStagingArea.OnSpellSlotSelected += OnStagingAreaItemSelected;
        _spellStagingArea.OnCraftSpellPressed += OnCraftSpellButtonPressed;
        _runicInventoryView.OnInventoryUpdated += OnInventoryUpdated;
        _spellStagingArea.OnSpellSlotsUpdated += OnSpellSlotsUpdated;
    }

    private void UnsubscribeToSubPanels() {
        _runicInventoryView.OnGridItemHighlighted -= OnInventoryItemHighlighted;
        _runicInventoryView.OnGridItemSelected -= OnInventoryItemSelected;
        _spellStagingArea.OnSpellSlotHighlighted -= OnStagingAreaItemHighlighted;
        _spellStagingArea.OnSpellSlotSelected -= OnStagingAreaItemSelected;
        _spellStagingArea.OnCraftSpellPressed -= OnCraftSpellButtonPressed;
        _runicInventoryView.OnInventoryUpdated -= OnInventoryUpdated;
        _spellStagingArea.OnSpellSlotsUpdated -= OnSpellSlotsUpdated;
    }

    public override void ClosePanel() {
        _spellStagingArea.ClearSpellComponentSlots();

        // close the panel
        base.ClosePanel();

        // remove listeners
        UnsubscribeToSubPanels();
    }

    public override void ChangePanel(UISubPanel neighbor, IntVector3 dir, bool hardLocked = false) {
        _runicInventoryView.SetFocus(_runicInventoryView == neighbor, hardLocked, dir);
        _spellStagingArea.SetFocus(_spellStagingArea == neighbor, hardLocked, dir);
    }

    private void OnInventoryItemHighlighted() {
        _spellComponentDescriptionView.UpdateDescription(_runicInventoryView.HighlightedItemId);
    }

    private void OnInventoryUpdated() {
        if (!_runicInventoryView.IsFocused) {
            return;
        }
        _spellComponentDescriptionView.UpdateDescription(_runicInventoryView.HighlightedItemId);
    }

    private void OnSpellSlotsUpdated(string itemId) {
        if (!_spellStagingArea.IsFocused) {
            return;
        }
        _spellComponentDescriptionView.UpdateDescription(itemId);
    }

    private void OnInventoryItemSelected() {
        IInventoryStorable inventoryStorable = InventoryRegistry.Instance.GetItemById(_runicInventoryView.HighlightedItemId);
        Spell_CastingMethod castingMethod = inventoryStorable as Spell_CastingMethod;
        if(castingMethod != null) {
            _spellStagingArea.SetCastingMethod(castingMethod);
            _runicInventoryView.RemoveItem(_runicInventoryView.HighlightedItemId, 1);
            _spellComponentDescriptionView.UpdateDescription(_runicInventoryView.HighlightedItemId);
            return;
        }
        Effect spellEffect = inventoryStorable as Effect;
        if(spellEffect != null) {
            if (_spellStagingArea.AddSpellEffect(spellEffect)) {
                _runicInventoryView.RemoveItem(_runicInventoryView.HighlightedItemId, 1);
                _spellComponentDescriptionView.UpdateDescription(_runicInventoryView.HighlightedItemId);
            }
            return;
        }
        SpellModifier spellModifier = inventoryStorable as SpellModifier;
        if(spellModifier != null) {
            if (_spellStagingArea.AddSpellModifier(spellModifier)) {
                _runicInventoryView.RemoveItem(_runicInventoryView.HighlightedItemId, 1);
                _spellComponentDescriptionView.UpdateDescription(_runicInventoryView.HighlightedItemId);
            }
        }
    }
    
    private void OnStagingAreaItemHighlighted(string itemId) {
        _spellComponentDescriptionView.UpdateDescription(itemId);
    }

    private void OnStagingAreaItemSelected(string itemId) {
        IInventoryStorable inventoryStorable = InventoryRegistry.Instance.GetItemById(itemId);
        if(inventoryStorable == null) {
            Debug.LogError("Spell component slot contains an invalid inventory itemId!");
            return;
        }
        _runicInventoryView.AddItem(itemId, 1);
        _spellStagingArea.RemoveHighlightedSpellComponent();
    }

    private void OnCraftSpellButtonPressed() {

    }
}
