using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Objects that may appear many times in world space
/// </summary>
public abstract class PooledObject : MonoBehaviour {

    [SerializeField] protected string _prefabId;
    public string PrefabId => _prefabId;

    [SerializeField] protected bool _inUse;
    public bool InUse => _inUse;

    public void SetPooledObjectPrefabId(string prefabId) {
        _prefabId = prefabId;
    }

    public virtual void DeactivatePooledObject() {

    }

    public virtual void ActivatePooledObject() {

    }
}
