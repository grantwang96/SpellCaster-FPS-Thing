using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerPhysics : MonoBehaviour {

    [SerializeField] private float _weight;

    private CharacterController characterController;

    private void Start() {
        characterController = GetComponent<CharacterController>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        if(hit.rigidbody != null) {
            Vector3 dir = hit.transform.position - (characterController.center + transform.position);
            hit.rigidbody.AddForce(dir.normalized * _weight, ForceMode.Impulse);
        }
    }
}
