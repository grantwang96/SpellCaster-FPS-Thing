using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCraftingStation : MonoBehaviour, IInteractable {

    [SerializeField] private bool _interactable = true;
    public bool Interactable { get { return _interactable; } }

    [SerializeField] private UIPanel _spellCraftingStationPanel;

    public void Detect() {
        
    }

    public void Interact(CharacterBehaviour character) {
        Debug.Log($"You are interacting with spell craft station: {this.name}");
        IInventory inventory = character.GetComponent<IInventory>();
        if(inventory == null) {
            return;
        }
        InventoryPanelInitData initData = new InventoryPanelInitData() {
            Inventory = inventory,
            Filter = new InventoryItemType[] {
                InventoryItemType.CASTINGMETHOD,
                InventoryItemType.SPELLEFFECT,
                InventoryItemType.SPELLMODIFIER
            }
        };
        UIManager.Instance.OpenUIPanel(_spellCraftingStationPanel, initData);
    }

    private void OnPanelClosed() {
        
    }
}
