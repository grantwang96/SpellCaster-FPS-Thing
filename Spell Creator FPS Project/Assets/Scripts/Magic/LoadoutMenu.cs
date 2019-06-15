using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class handles setting the player's loadout before play
/// </summary>
public class LoadoutMenu : UISubPanelParent {

    [SerializeField] private UISpellsInventoryGridContainer _spellsInventoryView;
    // [SerializeField] private UISpellDescriptionView _spellsDescriptionView; // This may not be a connected subpanel
    [SerializeField] private UILoadoutViewGridContainer _loadoutView;

    public override void Initialize(UIPanelInitData initData) {

        SpellsInventoryViewInitData viewInitData = new SpellsInventoryViewInitData();
        _spellsInventoryView.Initialize(viewInitData);
        // _spellsDescriptionView.Initialize(initData);

        _spellsInventoryView.SetActive(true, IntVector3.Zero);
        // _spellsDescriptionView.SetActive(false, IntVector3.Zero);

        _spellsInventoryView.OnSpellSelected += OnInventorySpellSelected;

        base.Initialize(initData);
    }

    // when a spell has been selected on the grid side of the panel
    private void OnInventorySpellSelected() {
        StorableSpell spell = PlayerInventory.SpellInventory.GetSpellByInstanceId(_spellsInventoryView.HighlightedSpellInstanceId);
        if(spell == null) {
            return;
        }
        Debug.Log($"Selected spell {spell.InstanceId}!");
        // display options menu to allow setting in loadout, renaming, or deleting
    }

    // when a spell has been selected on the loadout side of the panel
    private void OnLoadoutSpellSelected() {

    }

    protected override void CloseUIPanel() {
        base.CloseUIPanel();
    }
}