using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutStation : InteractableStation {

    [SerializeField] private string _loadOutPanelPrefabName;
    [SerializeField] private BoxCollider _collider;

    public override Vector3 InteractableCenter {
        get {
            return transform.position + _collider.center;
        }
    }
    
    protected override void OnPlayerInteract() {
        UIManager.Instance.OpenUIPanel(_loadOutPanelPrefabName);
    }
}
