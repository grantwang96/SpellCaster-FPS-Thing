using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Hurtbox : MonoBehaviour
{
    [SerializeField] private int _priority;
    [SerializeField] private float _damageScale = 1f;
    [SerializeField] private Damageable _owner;

    public event Action<HitData, float, int> OnHit;
    
    public void Hit(HitData data) {

        // determine if the hit should be successful

        // send hit message
        OnHit?.Invoke(data, _damageScale, _priority);
    }

    public bool CompareOwner(Damageable other) {
        return _owner == other;
    }
}
