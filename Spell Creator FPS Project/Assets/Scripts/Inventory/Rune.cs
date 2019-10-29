﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rune : MonoBehaviour, PooledObject, IInteractable, ILootable {

    [SerializeField] private string _prefabId;
    public string PrefabId => _prefabId;
    [SerializeField] private bool _inUse;
    public bool InUse => _inUse;
    [SerializeField] private bool _interactable = true;
    public bool Interactable { get { return _interactable; } }
    [SerializeField] private string _itemId; // item id contained in rune
    public string Id => _itemId;
    
    public string InteractableId { get; private set; }
    public Vector3 InteractableCenter => transform.position;

    [SerializeField] private float _spawnSpread;
    [SerializeField] private float _spawnForce;
    [SerializeField] private float _flySpeed;

    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Collider _collider;

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

    public void InteractPress(CharacterBehaviour character) {
        OnInteractAttempt?.Invoke();
        if (!Interactable) { return; }
        if(character == PlayerController.Instance) {
            _interactable = false;
            GameManager.GameManagerInstance.CurrentRunicInventory.AddItem(_itemId, 1);
            // PersistedInventory.RunicInventory.AddItem(_itemId, 1);
            OnInteractSuccess?.Invoke();
            StartCoroutine(FlyToTarget(character.BodyTransform));
        }
    }

    public void InteractHold(CharacterBehaviour character) {

    }

    public void Initialize() {
        
    }

    public void ReleaseFromChest(Vector3 force) {
        
    }

    public void ActivatePooledObject() {
        gameObject.SetActive(true);
        LevelManager.LevelManagerInstance.RegisterInteractable(this);
        Vector3 offset = Random.insideUnitCircle * _spawnSpread;
        offset = new Vector3(offset.x, 0f, offset.y);
        Vector3 dir = Vector3.up + offset;
        _collider.enabled = true;
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
        _rigidbody.AddForce(dir.normalized * _spawnForce, ForceMode.Impulse);
    }

    public void DeactivatePooledObject() {
        gameObject.SetActive(false);
        LevelManager.LevelManagerInstance.UnregisterInteractable(this);
    }

    private IEnumerator FlyToTarget(Transform target) {
        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;
        _collider.enabled = false;
        _interactable = false;
        float time = 0f;
        while (time < 1f) {
            time += Time.deltaTime * _flySpeed;
            transform.position = Vector3.Lerp(transform.position, target.position, time);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        DeactivatePooledObject();
        ObjectPool.Instance.ReturnUsedPooledObject(this);
    }
}
