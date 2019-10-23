using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WeaponCollider : MonoBehaviour {

    [SerializeField] private Damageable _owner;
    [SerializeField] private int _power;
    [SerializeField] private Element _element;
    
    void OnTriggerEnter(Collider other) {
        Damageable damageable = other.GetComponent<Damageable>();
        if (damageable != null && damageable != _owner) {
            damageable.TakeDamage(_power, _element);
        }
    }
}
