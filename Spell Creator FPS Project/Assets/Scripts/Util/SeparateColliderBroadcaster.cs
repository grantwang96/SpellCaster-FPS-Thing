using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach this to collider you want to listen to
/// </summary>
public class SeparateColliderBroadcaster : MonoBehaviour {

    public delegate void ColliderHitEvent(Collider coll);
    public event ColliderHitEvent TriggerEnter;
    public event ColliderHitEvent TriggerStay;
    public event ColliderHitEvent TriggerExit;

    private void OnTriggerEnter(Collider other) {
        TriggerEnter?.Invoke(other);
    }

    private void OnTriggerStay(Collider other) {
        TriggerStay?.Invoke(other);
    }

    private void OnTriggerExit(Collider other) {
        TriggerExit?.Invoke(other);
    }
}
