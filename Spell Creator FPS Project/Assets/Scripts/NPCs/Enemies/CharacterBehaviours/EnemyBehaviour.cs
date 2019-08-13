using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : NPCBehaviour, PooledObject {

    [SerializeField] private string _prefabId;
    public string PrefabId => _prefabId;
    [SerializeField] private bool _inUse;
    public bool InUse => _inUse;

    public delegate void SpawnEvent(EnemyBehaviour behaviour);
    public event SpawnEvent OnEnemySpawned;

    public void ActivatePooledObject() {
        gameObject.SetActive(true);
        OnEnemySpawned?.Invoke(this);
    }

    public void DeactivatePooledObject() {
        gameObject.SetActive(false);
    }

    protected override void OnDeath(bool isDead) {
        base.OnDeath(isDead);
        DeactivatePooledObject();
        ObjectPool.Instance?.ReturnUsedPooledObject(this);
    }
}