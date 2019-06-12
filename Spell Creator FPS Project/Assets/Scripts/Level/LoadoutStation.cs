using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutStation : MonoBehaviour, IInteractable {

    [SerializeField] private string _loadOutPanelPrefabName;
    public bool Interactable { get; private set; }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Detect() {

    }

    public void Interact(CharacterBehaviour character) {
        UIManager.Instance.OpenUIPanel(_loadOutPanelPrefabName);
    }

    private void OnPanelClosed() {

    }
}
