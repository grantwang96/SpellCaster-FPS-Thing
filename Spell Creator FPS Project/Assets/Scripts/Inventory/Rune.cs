using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rune : PooledObject, IInteractable, ILootable {

    [SerializeField] private bool _interactable = true;
    public bool Interactable { get { return _interactable; } }
    [SerializeField] private string _itemId; // item id contained in rune
    public string Id => _itemId;
    
    public string InteractableId { get; private set; }
    public Vector3 InteractableCenter => transform.position;

    [SerializeField] private MeshFilter _outerMeshFilter;
    [SerializeField] private MeshRenderer _outerMeshRenderer;

    // components that help display inner symbol
    [SerializeField] private MeshFilter _innerSymbolMeshFilter;
    [SerializeField] private MeshRenderer _innerSymbolMeshRenderer;

    public event InteractEvent OnInteractAttempt;
    public event InteractEvent OnInteractSuccess;

    private void Awake() {
        InteractableId = $"{GameplayValues.Level.RuneInstanceIdPrefix}{StringGenerator.RandomString(GameplayValues.Level.RuneInstanceIdSize)}";
    }

    public void Initialize(string itemId) {
        _itemId = itemId;
        InteractableId = $"{GameplayValues.Level.RuneInstanceIdPrefix}{StringGenerator.RandomString(GameplayValues.Level.RuneInstanceIdSize)}";
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
        OnInteractAttempt?.Invoke();
        if (!Interactable) { return; }
        if(character == GameplayController.Instance) {
            _interactable = false;
            PlayerInventory.RunicInventory.AddItem(_itemId, 1);
            OnInteractSuccess?.Invoke();
            DeactivatePooledObject();
        }
    }

    public void Initialize() {
        
    }

    public void ReleaseFromChest(Vector3 force) {
        
    }

    public override void ActivatePooledObject() {
        gameObject.SetActive(true);
        LevelManager.Instance.RegisterInteractable(this);
    }

    public override void DeactivatePooledObject() {
        base.DeactivatePooledObject();
        gameObject.SetActive(false);
        LevelManager.Instance.UnregisterInteractable(this);
    }
}
