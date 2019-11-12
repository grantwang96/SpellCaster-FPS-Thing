using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevelManager {

    List<ChestSpawn> ChestLocations { get; }

    void RegisterInteractable(IInteractable interactable);
    void UnregisterInteractable(IInteractable interactable);
    IInteractable GetInteractable(string interactableId);
}

public interface ICampaignLevelManager : ILevelManager {
    void RegisterRoom(Room room);
    void RegisterEnemySpawn(string id, EnemySpawn spawn);
    EnemySpawn GetEnemySpawnById(string id);
}

/// <summary>
/// Included in scene. Manages all location data of level.
/// </summary>
public class LevelManager : MonoBehaviour, ICampaignLevelManager {

    private const string EnemiesResourcePath = "Enemies";

    public static ILevelManager LevelManagerInstance { get; private set; }
    public static ICampaignLevelManager CampaignLevelManagerInstance { get; private set; }
    // list of all interactable objects
    private Dictionary<string, IInteractable> _interactables = new Dictionary<string, IInteractable>();
    // list of all treasure chest locations
    [SerializeField] private List<ChestSpawn> _chestLocations = new List<ChestSpawn>();
    public List<ChestSpawn> ChestLocations => _chestLocations;
    [SerializeField] private List<PooledObjectEntry> _enemyPrefabIds = new List<PooledObjectEntry>();
    // dictionary of doors and their respective keys
    // list of all enemy spawnpoints
    private Dictionary<string, EnemySpawn> _enemySpawnPoints = new Dictionary<string, EnemySpawn>();

    protected virtual void Awake() {
        LevelManagerInstance = this;
        CampaignLevelManagerInstance = this;
        RegisterEnemyPrefabs();
    }

    private void RegisterEnemyPrefabs() {
        for (int i = 0; i < _enemyPrefabIds.Count; i++) {
            ObjectPool.Instance.RegisterObjectToPool(EnemiesResourcePath, _enemyPrefabIds[i].Id, _enemyPrefabIds[i].Count);
        }
    }

    public void RegisterInteractable(IInteractable interactable) {
        if (_interactables.ContainsKey(interactable.InteractableId)) {
            return;
        }
        _interactables[interactable.InteractableId] = interactable;
    }

    public void UnregisterInteractable(IInteractable interactable) {
        _interactables.Remove(interactable.InteractableId);
    }

    public IInteractable GetInteractable(string interactableId) {
        if (!_interactables.ContainsKey(interactableId)) {
            Debug.LogError($"Could not find interactable with id: {interactableId}");
            return null;
        }
        return _interactables[interactableId];
    }

    public void RegisterRoom(Room room) {

    }

    public void RegisterEnemySpawn(string id, EnemySpawn spawn) {
        if (!_enemySpawnPoints.ContainsKey(id)) {
            _enemySpawnPoints.Add(id, spawn);
        }
    }

    public EnemySpawn GetEnemySpawnById(string id) {
        if (_enemySpawnPoints.ContainsKey(id)) {
            return _enemySpawnPoints[id];
        }
        return null;
    }
}
