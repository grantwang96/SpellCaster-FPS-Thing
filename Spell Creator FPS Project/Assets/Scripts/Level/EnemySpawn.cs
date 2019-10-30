using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour {

    [SerializeField] private string _id;
    [SerializeField] private string _enemyPrefabId;
    public string EnemyPrefabId => _enemyPrefabId;
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
        _finishedSpawning = true;
    }

    protected virtual EnemyBehaviour SpawnEnemyObject(string overrideUniqueId) {
        return NPCManager.Instance.SpawnPooledNPC(_enemyPrefabId, transform.position, overrideUniqueId);
    }
}
