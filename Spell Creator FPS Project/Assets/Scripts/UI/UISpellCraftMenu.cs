using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISpellCraftMenu : UIPanel {

    [SerializeField] private bool _isSpellCraftMode;

    [SerializeField] private InventoryView _inventoryView;

    public override void Initialize(UIPanelInitData initData) {
        InventoryPanelInitData inventoryData = initData as InventoryPanelInitData;
        _inventoryView?.Initialize(inventoryData);
    }

    protected override void Update() {
        base.Update();

    }

    private void ProcessInventory() {
        if(UIManager.Instance.CurrentPanel == this) {
            // do the inventory processing thing
        }
    }

    private void ProcessSpellCraftMenu() {

    }
}
