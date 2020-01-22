using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : NPCBehaviour, PooledObject {

    [SerializeField] private string _prefabId;
    public string PrefabId => name;
    [SerializeField] private bool _inUse;
    public bool InUse => _inUse;

    public string UniqueId => _id;

    public delegate void SpawnEvent(EnemyBehaviour behaviour);
    
    public void ActivatePooledObject(string uniqueId = "") {
        gameObject.SetActive(true);
        GenerateUniqueId(uniqueId);
        FireCharacterSpawnedEvent();
    }

    public void DeactivatePooledObject() {
        gameObject.SetActive(false);
    }

    protected override void OnDeath(bool isDead, Damageable dam) {
        base.OnDeath(isDead, dam);
        DeactivatePooledObject();
        PooledObjectManager.Instance?.ReturnPooledObject(PrefabId, this);
    }
}