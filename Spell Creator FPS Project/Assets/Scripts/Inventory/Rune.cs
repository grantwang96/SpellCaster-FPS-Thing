using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rune : PooledObject, IInteractable, ILootable {

    [SerializeField] private bool _interactable = true;
    public bool Interactable { get { return _interactable; } }
    [SerializeField] private string _itemId;
    public string Id => _itemId;

    [SerializeField] private MeshFilter _outerMeshFilter;
    [SerializeField] private MeshRenderer _outerMeshRenderer;

    // components that help display inner symbol
    [SerializeField] private MeshFilter _innerSymbolMeshFilter;
    [SerializeField] private MeshRenderer _innerSymbolMeshRenderer;

    public void Initialize(string itemId) {
        _itemId = itemId;
        IInventoryStorable storable = InventoryRegistry.Instance.GetItemById(_itemId);
        if(storable == null) {
            Debug.LogError($"Item with ID {_itemId} could not be found!");
            DeactivatePooledObject();
            return;
        }
        ActivatePooledObject();
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

    public void Initialize() {
        
    }

    public void ReleaseFromChest(Vector3 force) {
        
    }

    public override void ActivatePooledObject() {
        gameObject.SetActive(true);
    }
}
