using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCraftingStation : InteractableStation {
    
    [SerializeField] private string _spellCraftingStationPrefabName;
    [SerializeField] private BoxCollider _collider;

    public override Vector3 InteractableCenter {
        get {
            return transform.position + _collider.center;
        }
    }

    protected override void OnPlayerInteract() {
        UIManager.Instance.OpenUIPanel(_spellCraftingStationPrefabName);
    }
}
