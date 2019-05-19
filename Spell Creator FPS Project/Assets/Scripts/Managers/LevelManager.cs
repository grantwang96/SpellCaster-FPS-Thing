using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevelManager {
    List<ChestSpawn> ChestLocations { get; }
}

/// <summary>
/// Included in scene. Manages all location data of level.
/// </summary>
public class LevelManager : MonoBehaviour, ILevelManager {

    private static ILevelManager _instance;
    public static ILevelManager Instance => _instance;

    // list of all treasure chest locations
    [SerializeField] private List<ChestSpawn> _chestLocations = new List<ChestSpawn>();
    public List<ChestSpawn> ChestLocations => _chestLocations;
    // dictionary of doors and their respective keys
    // list of all enemy spawnpoints

    private void Awake() {
        _instance = this;
    }
}
