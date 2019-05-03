using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Objects that may appear many times in world space
/// </summary>
public abstract class PooledObject : MonoBehaviour {

    [SerializeField] protected bool _inUse;
    public bool InUse => _inUse;
}
