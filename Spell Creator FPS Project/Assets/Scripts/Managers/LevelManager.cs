using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevelManager {
    List<ChestSpawn> ChestLocations { get; }

    void RegisterInteractable(IInteractable interactable);
    void UnregisterInteractable(IInteractable interactable);
    IInteractable GetInteractable(string interactableId);

    void RegisterRoom(Room room);

    void RegisterEnemySpawn(EnemySpawn spawn);
}

/// <summary>
/// Included in scene. Manages all location data of level.
/// </summary>
public class LevelManager : MonoBehaviour, ILevelManager {

    public static ILevelManager Instance { get; private set; }
    // list of all interactable objects
    private Dictionary<string, IInteractable> _interactables = new Dictionary<string, IInteractable>();
    // list of all treasure chest locations
    [SerializeField] private List<ChestSpawn> _chestLocations = new List<ChestSpawn>();
    public List<ChestSpawn> ChestLocations => _chestLocations;
    // dictionary of doors and their respective keys
    // list of all enemy spawnpoints
    [SerializeField] private List<EnemySpawn> _enemySpawnPoints = new List<EnemySpawn>();

    private void Awake() {
        Instance = this;
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

    public void RegisterEnemySpawn(EnemySpawn spawn) {
        if (!_enemySpawnPoints.Contains(spawn)) {
            _enemySpawnPoints.Add(spawn);
        }
    }
}
