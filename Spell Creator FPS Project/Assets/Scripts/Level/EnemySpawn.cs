using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour {

    [SerializeField] private string _enemyPrefabId;
    public string EnemyPrefabId => _enemyPrefabId;
    private bool _finishedSpawning = false;

    private Room _room;

    public void InitializeSpawnPoint(Room room) {
        _room = room;
    }

    public virtual void SpawnPrefab() {
        if (_finishedSpawning) {
            return;
        }
        SpawnEnemy();
        _finishedSpawning = true;
    }

    protected virtual void SpawnEnemy() {
        NPCManager.Instance.SpawnPooledNPC(_enemyPrefabId, transform.position);
    }
}
