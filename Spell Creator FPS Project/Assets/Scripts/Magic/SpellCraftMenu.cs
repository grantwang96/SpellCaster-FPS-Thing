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
        _spellCraftManager = new SpellCraftManager();
        _spellStagingArea.Initialize();
        _runicInventoryView.Initialize(null);
        _runicInventoryView.SetActive(true, false, IntVector3.Zero);
        _spellStagingArea.SetActive(false, false, IntVector3.Zero);
        _runicInventoryView.OnGridItemHighlighted += OnInventoryItemHighlighted;
        _runicInventoryView.OnGridItemSelected += OnInventoryItemSelected;
        _spellStagingArea.OnSpellSlotHighlighted += OnStagingAreaItemHighlighted;
        _spellStagingArea.OnSpellSlotSelected += OnStagingAreaItemSelected;
        _spellStagingArea.OnCraftSpellPressed += OnCraftSpellButtonPressed;
        _runicInventoryView.OnInventoryUpdated += OnInventoryUpdated;
        _spellStagingArea.OnSpellSlotsUpdated += OnSpellSlotsUpdated;
        Debug.Log("Finished initializing spell craft menu");

        // add on hover events here(maybe limit if we're on PC or not)
    }

    protected override void CloseUIPanel() {
        // return any staged runes to the player
        if(_spellCraftManager.LoadedCastingMethod != null) {
            _runicInventoryView.AddItem(_spellCraftManager.LoadedCastingMethod.Id, 1);
        }
        foreach(Effect spellEffect in _spellCraftManager.LoadedSpellEffects) {
            _runicInventoryView.AddItem(spellEffect.Id, 1);
        }
        foreach(SpellModifier spellModifier in _spellCraftManager.LoadedSpellModifiers) {
            _runicInventoryView.AddItem(spellModifier.Id, 1);
        }
        _spellStagingArea.ClearSpellComponentSlots();

        // close the panel
        base.CloseUIPanel();

        // remove listeners
        _runicInventoryView.OnGridItemSelected -= OnInventoryItemSelected;
        _spellStagingArea.OnSpellSlotSelected -= OnStagingAreaItemSelected;
        _spellStagingArea.OnCraftSpellPressed -= OnCraftSpellButtonPressed;
    }

    public override void ChangePanel(UISubPanel neighbor, IntVector3 dir) {
        _runicInventoryView.SetActive(_runicInventoryView == neighbor, false, dir);
        _spellStagingArea.SetActive(_spellStagingArea == neighbor, false, dir);
    }

    private void OnInventoryItemHighlighted() {
        _spellComponentDescriptionView.UpdateDescription(_runicInventoryView.HighlightedItemId);
    }

    private void OnInventoryUpdated() {
        if (!_runicInventoryView.IsActive) {
            return;
        }
        _spellComponentDescriptionView.UpdateDescription(_runicInventoryView.HighlightedItemId);
    }

    private void OnSpellSlotsUpdated(string itemId) {
        if (!_spellStagingArea.IsActive) {
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
        PlayerInventory.SpellInventory.AddSpell(storableSpell);
        _spellCraftManager.ClearSpellComponents();
        _spellStagingArea.ClearSpellComponentSlots();
        
        // add this to loadout if loadout is not full
        for(int i = 0; i < GameplayValues.Magic.PlayerLoadoutMaxSize; i++) {
            if(PlayerInventory.SpellInventory.CurrentLoadout[i] == null) {
                PlayerInventory.SpellInventory.SetSpellInLoadout(storableSpell.InstanceId, i);
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
        UIManager.Instance.OpenUIPanel(UIManager.GenericMessageBoxPrefabId, messageBoxData);
        // cut view grid input listening

    }

    private void OpenLoadoutEditor() {
        UIManager.Instance.CloseUIPanel();
        UIManager.Instance.OpenUIPanel(_loadoutPrefabId);
    }

    private void CloseUseSpellNowDialog() {
        UIManager.Instance.CloseUIPanel();
    }
}
