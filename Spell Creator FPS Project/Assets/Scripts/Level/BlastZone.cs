using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastZone : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        Damageable damageable = other.GetComponent<Damageable>();
        if(damageable != null) {
            damageable.TakeDamage(9999999);
        }
    }
}
