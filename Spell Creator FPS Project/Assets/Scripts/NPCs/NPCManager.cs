using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INPCManager {

    EnemyBehaviour SpawnPooledNPC(string npcPrefabId, Vector3 position, Vector3 rotation, string uniqueId = "");
    EnemyBehaviour GetActiveNPC(string npcId);

    event Action<string, EnemyBehaviour> OnEnemySpawned;
    event Action<EnemyBehaviour> OnEnemyDefeated;
}

public class NPCManager : MonoBehaviour, INPCManager {

    public static INPCManager Instance { get; private set; }

    private Dictionary<string, EnemyBehaviour> _activeNPCs = new Dictionary<string, EnemyBehaviour>();

    public event Action<string, EnemyBehaviour> OnEnemySpawned;
    public event Action<EnemyBehaviour> OnEnemyDefeated;

    private void Awake() {
        Instance = this;
    }

    public EnemyBehaviour SpawnPooledNPC(string npcPrefabId, Vector3 position, Vector3 rotation, string uniqueId = "") {
        PooledObject pooledObject;
        if (!PooledObjectManager.Instance.UsePooledObject(npcPrefabId, out pooledObject)) {
            return null;
        }
        EnemyBehaviour enemy = pooledObject as EnemyBehaviour;
        if (enemy == null) {
            ErrorManager.LogError(nameof(NPCManager), $"Pooled object {pooledObject} was not of type {nameof(EnemyBehaviour)}");
            return null;
        }
        enemy.transform.position = position;
        enemy.transform.eulerAngles = rotation;
        enemy.ActivatePooledObject(uniqueId);
        _activeNPCs.Add(enemy.UniqueId, enemy);
        enemy.Damageable.OnDeath += OnEnemyDeath;
        OnEnemySpawned?.Invoke(enemy.Id, enemy);
        return enemy;
    }

    public EnemyBehaviour GetActiveNPC(string npcId) {
        if (_activeNPCs.ContainsKey(npcId)) {
            return _activeNPCs[npcId];
        }
        return null;
    }

    private void OnEnemyDeath(bool isDead, Damageable damageable) {
        damageable.OnDeath -= OnEnemyDeath;
        EnemyBehaviour enemy = damageable.GetComponent<EnemyBehaviour>();
        if(enemy == null) {
            return;
        }
        if (_activeNPCs.ContainsKey(enemy.Id)) {
            OnEnemyDefeated?.Invoke(_activeNPCs[enemy.Id]);
            _activeNPCs.Remove(enemy.Id);
        }
    }
}
