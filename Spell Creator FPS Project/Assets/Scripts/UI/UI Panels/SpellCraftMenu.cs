using System.Collections;
using System.Collections.Generic;
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

    public ISpellCraftManager SpellCraftManager => _spellCraftManager;
    private SpellCraftManager _spellCraftManager;

    public override void Initialize(UIPanelInitData initData = null) {
        base.Initialize(initData);
        _spellCraftManager = new SpellCraftManager();
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
        // return any staged runes to the player
        if (_spellCraftManager.LoadedCastingMethod != null) {
            _runicInventoryView.AddItem(_spellCraftManager.LoadedCastingMethod.Id, 1);
        }
        foreach (Effect spellEffect in _spellCraftManager.LoadedSpellEffects) {
            _runicInventoryView.AddItem(spellEffect.Id, 1);
        }
        foreach (SpellModifier spellModifier in _spellCraftManager.LoadedSpellModifiers) {
            _runicInventoryView.AddItem(spellModifier.Id, 1);
        }
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
            if(_spellCraftManager.LoadedCastingMethod != null) {
                _runicInventoryView.AddItem(_spellCraftManager.LoadedCastingMethod.Id, 1);
            }
            _runicInventoryView.RemoveItem(_runicInventoryView.HighlightedItemId, 1);
            _spellCraftManager.SetCastingMethod(castingMethod);
            _spellStagingArea.SetUICastingMethod(castingMethod);
            return;
        }
        Effect spellEffect = inventoryStorable as Effect;
        if(spellEffect != null) {
            if (_spellCraftManager.LoadedSpellEffects.Contains(spellEffect)) {
                // do failed effect here
                Debug.Log($"Already contains component {inventoryStorable.Id}");
                return;
            }
            _runicInventoryView.RemoveItem(_runicInventoryView.HighlightedItemId, 1);
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
            _runicInventoryView.RemoveItem(_runicInventoryView.HighlightedItemId, 1);
            _spellCraftManager.AddSpellModifier(spellModifier);
            _spellStagingArea.AddUISpellModifier(spellModifier);
        }
        _spellComponentDescriptionView.UpdateDescription(_runicInventoryView.HighlightedItemId);
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

        _spellCraftManager.RemoveComponentFromSpell(itemId);
        _spellStagingArea.RemoveHighlightedSpellComponent();
    }

    private void OnCraftSpellButtonPressed() {
        if(_spellCraftManager.LoadedCastingMethod == null) {
            return;
        }
        if(_spellCraftManager.LoadedSpellEffects.Count == 0) {
            return;
        }
        StorableSpell storableSpell = _spellCraftManager.GenerateSpell();
        storableSpell.SetName(_spellStagingArea.SpellName);
        GameManager.GameManagerInstance.CurrentSpellInventory.AddSpell(storableSpell);
        _spellCraftManager.ClearSpellComponents();
        _spellStagingArea.ClearSpellComponentSlots();
        
        // add this to loadout if loadout is not full
        for(int i = 0; i < GameplayValues.Magic.PlayerLoadoutMaxSize; i++) {
            if(GameManager.GameManagerInstance.CurrentSpellInventory.CurrentLoadout[i] == null) {
                GameManager.GameManagerInstance.CurrentSpellInventory.SetSpellInLoadout(storableSpell.InstanceId, i);
                return;
            }
        }
        // ask the player if they wish to use this spell now
        List<ButtonActionData> buttonActionDatas = new List<ButtonActionData>();
        buttonActionDatas.Add(
            new ButtonActionData() {
                ButtonId = GameplayValues.UI.GenericButtonIdYes,
                ButtonText = "Yes",
                Action = OpenLoadoutEditor
            }
        );
        buttonActionDatas.Add(
            new ButtonActionData() {
                ButtonId = GameplayValues.UI.GenericButtonIdNo,
                ButtonText = "No",
                Action = CloseUseSpellNowDialog
            }
        );
        GenericMessageBoxInitData messageBoxData =
            new GenericMessageBoxInitData("Use Spell Now?", "Would you like to use this spell in your current loadout?", buttonActionDatas);
        UIPanelManager.Instance.OpenUIPanel(UIPanelManager.GenericMessageBoxPrefabId, messageBoxData);
        // cut view grid input listening

    }

    private void OpenLoadoutEditor() {
        CloseUseSpellNowDialog();
        CoroutineGod.Instance.ExecuteAfterOneFrame(OpenLoadoutPrefab);
    }

    private void OpenLoadoutPrefab() {
        UIPanelManager.Instance.OpenUIPanel(_loadoutPrefabId);
    }

    private void CloseUseSpellNowDialog() {
        UIPanelManager.Instance.CloseUIPanel();
    }
}
