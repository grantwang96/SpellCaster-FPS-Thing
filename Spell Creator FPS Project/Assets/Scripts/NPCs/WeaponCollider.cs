using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WeaponCollider : MonoBehaviour {

    [SerializeField] private IDamageable _owner;
    [SerializeField] private int _power;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other) {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && damageable != _owner) {
            damageable.TakeDamage(_power);
        }
    }
}
