using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class handles setting the player's loadout before play
/// </summary>
public class LoadoutMenu : UISubPanelParent {

    [SerializeField] private UISpellsInventoryGridContainer _spellsInventoryView;
    [SerializeField] private UILoadoutViewGridContainer _loadoutView;
    [SerializeField] private UISpellOptionsMenu _spellOptionsMenu;

    // these will probably not be interactable subpanels
    [SerializeField] private UISpellDescriptionView _loadoutSpellDescriptionView;
    [SerializeField] private UISpellDescriptionView _inventorySpellDescriptionView;

    // public Action<string> SpellSelectedAction;

    private LoadoutMenuState CurrentState;

    #region INITIALIZATION STUFF
    public override void Initialize(UIPanelInitData initData) {
        ChangeState(new LoadoutMenuState());
        InitializeViewGridContainers();
        InitializeSpellOptionsMenu();
        InitializeSpellDescriptionViews();
        // SpellSelectedAction = OpenSpellOptionsMenu;

        base.Initialize(initData);
    }

    private void InitializeSpellDescriptionViews() {
        StorableSpell firstLoadoutSpell = PlayerInventory.SpellInventory.CurrentLoadout[0];
        string id = firstLoadoutSpell == null ? string.Empty : firstLoadoutSpell.InstanceId;
        SpellDescriptionViewInitData loadoutSpellInitData = new SpellDescriptionViewInitData(id);
        _loadoutSpellDescriptionView.SetVisible(true);
        _loadoutSpellDescriptionView.Initialize(loadoutSpellInitData);
        _inventorySpellDescriptionView.SetVisible(false);
        _inventorySpellDescriptionView.Initialize(null);
    }

    private void InitializeSpellOptionsMenu() {
        _spellOptionsMenu.Initialize(null);
        _spellOptionsMenu.SetFocus(false, true, IntVector3.Zero);
        _spellOptionsMenu.SetVisible(false);
    }
    
    private void InitializeViewGridContainers() {
        SpellsInventoryViewInitData viewInitData = new SpellsInventoryViewInitData();
        _spellsInventoryView.Initialize(viewInitData);
        _loadoutView.Initialize(null);
        _loadoutView.SetVisible(true);
        ChangePanel(_loadoutView, IntVector3.Zero);

        _loadoutView.OnGridItemHighlighted += LoadoutView_OnLoadoutSpellHighlighted;
        _loadoutView.OnGridItemSelected += LoadoutView_OnLoadoutSpellSelected;
        _spellsInventoryView.OnGridItemHighlighted += SpellsInventoryView_OnInventorySpellHighlighted;
        _spellsInventoryView.OnGridItemSelected += SpellsInventoryView_OnInventorySpellSelected;
    }
    #endregion

    #region VIEWGRIDCONTAINER LISTENERS
    // when a spell has been selected on the grid side of the panel
    private void SpellsInventoryView_OnInventorySpellSelected() {
        CurrentState.OnSpellInventoryItemSelected(_spellsInventoryView.HighlightedItemId);
    }

    private void SpellsInventoryView_OnInventorySpellHighlighted() {
        _inventorySpellDescriptionView.UpdateContainedSpell(_spellsInventoryView.HighlightedItemId);
        CurrentState.OnSpellInventoryItemHighlighted(_spellsInventoryView.HighlightedItemId);
    }

    private void LoadoutView_OnLoadoutSpellHighlighted() {
        _loadoutSpellDescriptionView.UpdateContainedSpell(_loadoutView.HighlightedItemId);
        CurrentState.OnLoadoutItemHighlighted(_loadoutView.HighlightedItemId);
    }

    // when a spell has been selected on the loadout side of the panel
    private void LoadoutView_OnLoadoutSpellSelected() {
        CurrentState.OnLoadoutItemSelected(_loadoutView.HighlightedItemId);
    }
    #endregion

    private void ChangeState(LoadoutMenuState nextState) {
        CurrentState = nextState;
        CurrentState.Enter(this);
    }

    public override void ChangePanel(UISubPanel subPanel, IntVector3 dir, bool hardLocked = false) {
        _spellsInventoryView.SetFocus(subPanel == _spellsInventoryView, hardLocked, dir);
        _loadoutView.SetFocus(subPanel == _loadoutView, hardLocked, dir);
        _inventorySpellDescriptionView.UpdateContainedSpell(_spellsInventoryView.HighlightedItemId);
        _inventorySpellDescriptionView.SetVisible(subPanel == _spellsInventoryView);
        _spellOptionsMenu.SetFocus(subPanel == _spellOptionsMenu, hardLocked, dir);
    }

    protected override void CloseUIPanel() {

        _loadoutView.OnGridItemHighlighted -= LoadoutView_OnLoadoutSpellHighlighted;
        _loadoutView.OnGridItemSelected -= LoadoutView_OnLoadoutSpellSelected;

        _spellsInventoryView.OnGridItemHighlighted -= SpellsInventoryView_OnInventorySpellHighlighted;
        _spellsInventoryView.OnGridItemSelected -= SpellsInventoryView_OnInventorySpellSelected;

        base.CloseUIPanel();
    }

    // Player is focused on loadout menu
    private class LoadoutMenuState {

        protected LoadoutMenu _loadoutMenu;

        public virtual void Enter(LoadoutMenu loadoutMenu) {
            _loadoutMenu = loadoutMenu;
        }

        public virtual void OnLoadoutItemSelected(string spellInstanceId) {
            bool hasSpell = PlayerInventory.SpellInventory.HasSpellByInstanceId(spellInstanceId);
            _loadoutMenu._spellOptionsMenu.SetVisible(true);
            _loadoutMenu._spellOptionsMenu.SetValue(new SpellOptionsInitData() {
                ButtonDatas = GenerateOptionMenuButtonActionDatas(hasSpell)
            });
            _loadoutMenu.ChangePanel(_loadoutMenu._spellOptionsMenu, IntVector3.Zero, true);
        }

        private List<ButtonActionData> GenerateOptionMenuButtonActionDatas(bool hasSpell) {
            List<ButtonActionData> buttonDatas = new List<ButtonActionData>();

            ButtonActionData changeSpellButtonData = new ButtonActionData() {
                ButtonId = "CHANGE_SPELL_BTN",
                ButtonText = "Change Spell",
                Action = OnChangeSpellSelected
            };
            buttonDatas.Add(changeSpellButtonData);
            if (hasSpell) {
                ButtonActionData renameSpellButtonData = new ButtonActionData() {
                    ButtonId = "RENAME_SPELL_BTN",
                    ButtonText = "Rename Spell",
                    Action = OnRenameSpellSelected
                };
                buttonDatas.Add(renameSpellButtonData);
            }
            ButtonActionData cancelButtonData = new ButtonActionData() {
                ButtonId = "CANCEL_BTN",
                ButtonText = "Cancel",
                Action = OnCancelSelected
            };
            buttonDatas.Add(cancelButtonData);

            return buttonDatas;
        }

        public virtual void OnLoadoutItemHighlighted(string spellInstanceId) {

        }

        public virtual void OnSpellInventoryItemSelected(string spellInstanceId) {

        }

        public virtual void OnSpellInventoryItemHighlighted(string spellInstanceId) {

        }

        #region SPELL OPTIONS MENU LISTENERS

        private void OnChangeSpellSelected() {
            _loadoutMenu.ChangePanel(_loadoutMenu._spellsInventoryView, IntVector3.Zero, true);
            _loadoutMenu._spellOptionsMenu.SetVisible(false);
            _loadoutMenu._spellsInventoryView.SetVisible(true);
            _loadoutMenu._loadoutView.SetVisible(false);
            _loadoutMenu.ChangeState(new LoadoutMenu_OptionMenuState());
        }

        private void OnRenameSpellSelected() {
            // TODO
        }

        private void OnCancelSelected() {
            _loadoutMenu.ChangePanel(_loadoutMenu._loadoutView, IntVector3.Zero, false);
            _loadoutMenu._spellsInventoryView.SetVisible(false);
            _loadoutMenu._spellOptionsMenu.SetVisible(false);
        }

        #endregion

    }

    // player is focused on option menu OR spell inventory menu
    private class LoadoutMenu_OptionMenuState : LoadoutMenuState {

        private StorableSpell _selectedSpell;

        public override void Enter(LoadoutMenu loadoutMenu) {
            base.Enter(loadoutMenu);
            _selectedSpell = PlayerInventory.SpellInventory.GetSpellByInstanceId(loadoutMenu._loadoutView.HighlightedItemId);
        }

        // performs the swap action
        public override void OnSpellInventoryItemSelected(string spellInstanceId) {
            bool hasSpell = PlayerInventory.SpellInventory.HasSpellByInstanceId(spellInstanceId);
            string instanceId = hasSpell ? spellInstanceId : string.Empty;
            StorableSpell[] loadout = PlayerInventory.SpellInventory.CurrentLoadout;
            for (int i = 0; i < loadout.Length; i++) {
                if (loadout[i] == _selectedSpell) {
                    StorableSpell spell = PlayerInventory.SpellInventory.GetSpellByInstanceId(spellInstanceId);
                    if(ArrayHelper.Contains(loadout, spell)) {
                        // do not let the player set the another slot with the same spell
                        return;
                    }
                    SetLoadoutSpell(instanceId, i);
                    break;
                }
            }
            PlayerInventory.SpellInventory.SoftRefresh();
            _loadoutMenu.ChangeState(new LoadoutMenuState());
        }

        private void SetLoadoutSpell(string spellInstanceId, int index) {
            PlayerInventory.SpellInventory.SetSpellInLoadout(spellInstanceId, index);
            _loadoutMenu.ChangePanel(_loadoutMenu._loadoutView, IntVector3.Zero, false);
            _loadoutMenu._loadoutView.SetVisible(true);
            _loadoutMenu._spellsInventoryView.SetVisible(false);
        }
    }
}