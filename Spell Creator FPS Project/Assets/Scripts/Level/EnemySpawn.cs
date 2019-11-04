using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour {

    [SerializeField] private string _id;
    [SerializeField] private string _enemyPrefabId;
    [SerializeField] private int _enemySpawnLimit;
    public string EnemyPrefabId => _enemyPrefabId;
    private int _spawnedEnemies = 0;
    private bool _finishedSpawning = false;

    private Room _room;

    private void Start() {
        LevelManager.CampaignLevelManagerInstance.RegisterEnemySpawn(_id, this);
    }

    public void InitializeSpawnPoint(Room room) {
        _room = room;
    }

    public virtual void SpawnNPC(BrainStateTransitionId transitionId = BrainStateTransitionId.Idle, float time = 0f, string overrideUniqueId = "") {
        if (_finishedSpawning) {
            return;
        }
        EnemyBehaviour enemy = SpawnEnemyObject(overrideUniqueId);
        enemy.ChangeBrainState(transitionId, time);
        _spawnedEnemies++;
        if(_enemySpawnLimit > 0 && _spawnedEnemies == _enemySpawnLimit) {
            _finishedSpawning = true;
        }
    }

    protected virtual EnemyBehaviour SpawnEnemyObject(string overrideUniqueId) {
        return NPCManager.Instance.SpawnPooledNPC(_enemyPrefabId, transform.position, transform.eulerAngles, overrideUniqueId);
    }
}
