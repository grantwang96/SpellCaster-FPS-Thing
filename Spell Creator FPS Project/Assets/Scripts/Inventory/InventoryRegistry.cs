﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryRegistry {

    IInventoryStorable GetItemById(string id);
}

public class InventoryRegistry : MonoBehaviour, IInventoryRegistry {
    
    public static IInventoryRegistry Instance { get; protected set;}

    [SerializeField] private string[] _resourceLocations;
    private Dictionary<string, IInventoryStorable> _inventoryRegistry = new Dictionary<string, IInventoryStorable>();

	// Use this for initialization
	private void Awake () {
        if (Instance != null && Instance != this) {
            Debug.LogError($"INVENTORY REGISTRY INSTANCE ALREADY CREATED!");
            return;
        }
        Instance = this;
        LoadAllStorableObjects();
        Debug.Log($"Inventory Registry loaded! Size: {_inventoryRegistry.Count}");
    }

    private void OnDestroy() {
        if (Instance != null && Instance == this) {
            Instance = null;
        }
    }

    private void LoadAllStorableObjects() {
        for(int i = 0; i < _resourceLocations.Length; i++) {
            LoadStorableObjectFromFolder(_resourceLocations[i]);
        }
    }

    private void LoadStorableObjectFromFolder(string location) {
        Object[] objs = Resources.LoadAll(location, typeof(IInventoryStorable));
        for(int i = 0; i < objs.Length; i++) {
            IInventoryStorable inventoryStorable = objs[i] as IInventoryStorable;
            if (inventoryStorable == null) {
                Debug.Log($"Object is not IInventoryStorable! Skipping...");
                continue;
            }
            if (_inventoryRegistry.ContainsKey(inventoryStorable.Id)) {
                Debug.Log($"Inventory Registry already contains key {inventoryStorable.Id}!");
                continue;
            }
            Debug.Log($"Registering inventory storable ({inventoryStorable.ToString()}) with ID {inventoryStorable.Id}");
            _inventoryRegistry.Add(inventoryStorable.Id, inventoryStorable);
        }
    }

    public IInventoryStorable GetItemById(string id) {
        IInventoryStorable storable;
        if(_inventoryRegistry.TryGetValue(id, out storable)) {
            return storable;
        }
        return null;
    }
}