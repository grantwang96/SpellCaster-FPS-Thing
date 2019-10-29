using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WeaponCollider : MonoBehaviour {

    [SerializeField] private Damageable _owner;
    [SerializeField] private Effect[] _effects;
    [SerializeField] private float _powerScale = 1f;
    
    void OnTriggerEnter(Collider other) {
        Damageable damageable = other.GetComponent<Damageable>();
        if (damageable != null && damageable != _owner) {
            Vector3 dir = damageable.transform.position - transform.position;
            dir = dir.normalized;
            TriggerEffects(damageable, dir);
        }
    }

    private void TriggerEffects(Damageable target, Vector3 direction) {
        for(int i = 0; i < _effects.Length; i++) {
            _effects[i].TriggerEffect(_owner, direction, _powerScale, transform.position, target);
        }
    }
}
