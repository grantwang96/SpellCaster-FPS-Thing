using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestSpawn : MonoBehaviour, ISpawnPoint {

    [SerializeField] private Chest _chest;
    [SerializeField] private ChestType _chestType;
    public ChestType ChestType => _chestType;

    public void SpawnPrefab(string id) {
        _chest.Initialize(ChestType, id);
        _chest.gameObject.SetActive(true);
    }
}
