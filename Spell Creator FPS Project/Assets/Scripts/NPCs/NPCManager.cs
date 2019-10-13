using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INPCManager {

    EnemyBehaviour SpawnPooledNPC(string npcPrefabId, Vector3 position);

    event Action<string, NPCBehaviour> OnNPCSpawned;
    event Action<NPCBehaviour> OnNPCDefeated;
}

public class NPCManager : MonoBehaviour, INPCManager {

    public static INPCManager Instance { get; private set; }

    private List<NPCBehaviour> _activeNPCs = new List<NPCBehaviour>();

    public event Action<string, NPCBehaviour> OnNPCSpawned;
    public event Action<NPCBehaviour> OnNPCDefeated;

    private void Awake() {
        Instance = this;
    }

    public EnemyBehaviour SpawnPooledNPC(string npcPrefabId, Vector3 position) {
        PooledObject pooledObject = ObjectPool.Instance.UsePooledObject(npcPrefabId);
        EnemyBehaviour enemy = pooledObject as EnemyBehaviour;
        if (enemy == null) {
            ErrorManager.LogError(nameof(ArenaManager), $"Pooled object {pooledObject} was not of type {nameof(EnemyBehaviour)}");
            return null;
        }
        enemy.transform.position = position;
        enemy.ActivatePooledObject();
        _activeNPCs.Add(enemy);
        enemy.Damageable.OnDeath += OnNPCDeath;
        OnNPCSpawned?.Invoke(enemy.Id, enemy);
        return enemy;
    }

    private void OnNPCDeath(bool isDead, Damageable damageable) {
        damageable.OnDeath -= OnNPCDeath;
        NPCBehaviour npc = damageable.GetComponent<NPCBehaviour>();
        if(npc == null) {
            return;
        }
        if (_activeNPCs.Contains(npc)) {
            OnNPCDefeated?.Invoke(npc);
        }
    }
}
