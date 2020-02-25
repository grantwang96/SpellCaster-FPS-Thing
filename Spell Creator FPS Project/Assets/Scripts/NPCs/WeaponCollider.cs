using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WeaponCollider : MonoBehaviour {

    [SerializeField] private Damageable _owner;
    [SerializeField] private Effect[] _effects;
    [SerializeField] private float _powerScale = 1f;
    [SerializeField] private Vector3 _knockBackDir;
    
    void OnTriggerEnter(Collider other) {
        Damageable damageable = other.GetComponent<Damageable>();
        if (damageable != null && damageable != _owner) {
            TriggerEffects(damageable);
        }
    }

    private void TriggerEffects(Damageable target) {
        Vector3 direction = _owner.Root.TransformDirection(_knockBackDir);
        for(int i = 0; i < _effects.Length; i++) {
            _effects[i].TriggerEffect(_owner, direction, _powerScale, transform.position, target);
        }
    }
}
