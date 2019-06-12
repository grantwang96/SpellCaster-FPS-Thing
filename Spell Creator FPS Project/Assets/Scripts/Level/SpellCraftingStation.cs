using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCraftingStation : MonoBehaviour, IInteractable {

    [SerializeField] private bool _interactable = true;
    public bool Interactable { get { return _interactable; } }

    [SerializeField] private string _spellCraftingStationPrefabName;

    public void Detect() {
        
    }

    public void Interact(CharacterBehaviour character) {
        UIManager.Instance.OpenUIPanel(_spellCraftingStationPrefabName);
    }

    private void OnPanelClosed() {
        
    }
}
