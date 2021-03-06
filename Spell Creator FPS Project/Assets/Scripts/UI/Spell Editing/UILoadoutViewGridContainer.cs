﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILoadoutViewGridContainer : UIInventoryViewGridContainer {

    // View cell prefab
    [SerializeField] private SpellViewCell _spellViewCellPrefab;

    private StorableSpell[] _loadout = new StorableSpell[GameplayValues.Magic.PlayerLoadoutMaxSize];

    public override void Initialize(UIPanelInitData initData) {
        base.Initialize(initData);
        GameManager.GameManagerInstance.CurrentSpellInventory.OnLoadoutDataUpdated += SpellInventory_OnLoadoutDataUpdated;
        GenerateViewCells();
        SpellInventory_OnLoadoutDataUpdated(GameManager.GameManagerInstance.CurrentSpellInventory.CurrentLoadout);
    }

    private void SpellInventory_OnLoadoutDataUpdated(StorableSpell[] Loadout) {
        for(int i = 0; i < _loadout.Length; i++) {
            _loadout[i] = Loadout[i];
        }
        UpdateViewCells();
    }

    protected override void SetGridInteractableItem(int x, int y) {
        SpellViewCellData initData = new SpellViewCellData(x, y);
        if (_loadout[x] == null) {
            _mainInventoryGrid.SetInteractableItem(x, y, initData);
            return;
        }
        StorableSpell currentSpell = _loadout[x];
        initData.Id = currentSpell.InstanceId;
        initData.SetValue(currentSpell);
        _mainInventoryGrid.SetInteractableItem(x, y, initData);
    }
}
