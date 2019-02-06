using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effect/Transmutation")]
public class Transmutation : StatusEffect {

    [SerializeField] private Damageable replacementObjectPrefab;
    private const string ReplacementObjectName = "TransmuteObject";

    public override void OnAddEffect(Damageable damageable, int power) {

        if(damageable.name == ReplacementObjectName) {
            return;
        }

        CharacterBehaviour characterBehaviour = damageable.GetComponent<CharacterBehaviour>();
        if(characterBehaviour != null) {
            characterBehaviour.BodyTransform.gameObject.SetActive(false);
            CharacterMoveController characterMove = characterBehaviour.GetComponent<CharacterMoveController>();
            characterMove.CharacterController.enabled = false;
            characterMove.enabled = false;
        } else {
            MeshRenderer meshRenderer = damageable.GetComponent<MeshRenderer>();
            if(meshRenderer != null) {
                meshRenderer.enabled = false;
            }
            Collider collider = damageable.GetComponent<Collider>();
            if(collider != null) {
                collider.enabled = false;
            }
            Rigidbody rigidbody = damageable.GetComponent<Rigidbody>();
            if (rigidbody != null) {
                rigidbody.useGravity = false;
            }
        }
        Damageable replacementObject = Instantiate(replacementObjectPrefab, damageable.transform.position, damageable.transform.rotation);
        replacementObject.parentDamageable = damageable;
        replacementObject.gameObject.name = ReplacementObjectName;
        replacementObject.transform.parent = damageable.transform;
    }

    public override void OnRemoveEffect(Damageable damageable) {

        Transform transmuteObject = damageable.transform.Find(ReplacementObjectName);
        if (transmuteObject) {
            damageable.transform.position = transmuteObject.position;
            Destroy(transmuteObject.gameObject);
        }

        CharacterBehaviour characterBehaviour = damageable.GetComponent<CharacterBehaviour>();
        if (characterBehaviour != null) {
            characterBehaviour.BodyTransform.gameObject.SetActive(true);
            CharacterMoveController characterMove = characterBehaviour.GetComponent<CharacterMoveController>();
            characterMove.enabled = true;
            characterMove.CharacterController.enabled = true;
        } else {
            MeshRenderer meshRenderer = damageable.GetComponent<MeshRenderer>();
            if (meshRenderer != null) {
                meshRenderer.enabled = true;
            }
            Collider collider = damageable.GetComponent<Collider>();
            if (collider != null) {
                collider.enabled = true;
            }
            Rigidbody rigidbody = damageable.GetComponent<Rigidbody>();
            if(rigidbody != null) {
                rigidbody.useGravity = true;
            }
        }
    }
}
