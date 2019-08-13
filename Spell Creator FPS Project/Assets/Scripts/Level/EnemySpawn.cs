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

    public virtual EnemyBehaviour SpawnPrefab() {
        if (_finishedSpawning) {
            return null;
        }
        EnemyBehaviour enemy = SpawnEnemy();
        _finishedSpawning = true;
        return enemy;
    }

    protected virtual EnemyBehaviour SpawnEnemy() {
        PooledObject pooledObject = ObjectPool.Instance?.UsePooledObject(_enemyPrefabId);
        if (pooledObject == null) {
            Debug.LogError($"[{nameof(EnemySpawn)}] \"{name}\" was not able to retrieve prefab with ID {_enemyPrefabId}!");
            return null;
        }
        EnemyBehaviour enemy = pooledObject as EnemyBehaviour;
        if (enemy == null) {
            Debug.LogError($"[{nameof(EnemySpawn)}] Object received was not of type {nameof(EnemyBehaviour)}");
            return null;
        }
        enemy.transform.position = transform.position;
        enemy.ActivatePooledObject();
        return enemy;
    }
}
