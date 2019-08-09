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
        PooledObject pooledObject = ObjectPool.Instance?.UsePooledObject(_enemyPrefabId);
        if (pooledObject == null) {
            Debug.LogError($"[{nameof(EnemySpawn)}] \"{name}\" was not able to retrieve prefab with ID {_enemyPrefabId}!");
            return;
        }
        EnemyBehaviour enemy = pooledObject as EnemyBehaviour;
        if (enemy == null) {
            Debug.LogError($"[{nameof(EnemySpawn)}] Object received was not of type {nameof(EnemyBehaviour)}");
            return;
        }
        enemy.transform.position = transform.position;
        enemy.ActivatePooledObject();
    }
}
