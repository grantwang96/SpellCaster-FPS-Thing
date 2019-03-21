using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rune : MonoBehaviour, IInteractable {

    [SerializeField] private bool _interactable = true;
    public bool Interactable { get { return _interactable; } }
    [SerializeField] private string _itemId;

    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshRenderer _meshRenderer;

    // components that help display inner symbol
    [SerializeField] private MeshFilter _innerSymbolMF;
    [SerializeField] private MeshRenderer _innerSymbolMR;

    public void Initialize(string itemId) {
        _itemId = itemId;
        // initialize visuals based on item id
    }

    public void Detect() {
        
    }

    public void Interact(CharacterBehaviour character) {
        if (!Interactable) { return; }
        if(character == GameplayController.Instance) {
            _interactable = false;
            PlayerInventory.RunicInventory.AddItem(_itemId, 1);
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
