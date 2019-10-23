using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : NPCBehaviour, PooledObject {

    [SerializeField] private string _prefabId;
    public string PrefabId => name;
    [SerializeField] private bool _inUse;
    public bool InUse => _inUse;

    public delegate void SpawnEvent(EnemyBehaviour behaviour);
    
    public void ActivatePooledObject() {
        gameObject.SetActive(true);
        GenerateUniqueId();
        ChangeBrainState(_startingState);
        FireCharacterSpawnedEvent();
    }

    public void DeactivatePooledObject() {
        gameObject.SetActive(false);
    }

    protected override void OnDeath(bool isDead, Damageable dam) {
        base.OnDeath(isDead, dam);
        DeactivatePooledObject();
        ObjectPool.Instance?.ReturnUsedPooledObject(this);
    }
}