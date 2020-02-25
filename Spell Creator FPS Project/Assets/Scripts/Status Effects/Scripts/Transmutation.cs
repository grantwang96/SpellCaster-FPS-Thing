using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effect/Transmutation")]
public class Transmutation : StatusEffect {

    [SerializeField] private GameObject replacementObjectPrefab;
    private const string ReplacementObjectName = "TransmuteObject";

    public override void OnAddEffect(Damageable damageable, int power) {

        if(damageable.Root.name == ReplacementObjectName) {
            return;
        }

        CharacterBehaviour characterBehaviour = damageable.Root.GetComponent<CharacterBehaviour>();
        if(characterBehaviour != null) {
            characterBehaviour.BodyTransform.gameObject.SetActive(false);
            CharacterMoveController characterMove = characterBehaviour.GetComponent<CharacterMoveController>();
            characterMove.CharacterController.enabled = false;
            characterMove.enabled = false;
        } else {
            MeshRenderer meshRenderer = damageable.Root.GetComponent<MeshRenderer>();
            if(meshRenderer != null) {
                meshRenderer.enabled = false;
            }
            Collider collider = damageable.Root.GetComponent<Collider>();
            if(collider != null) {
                collider.enabled = false;
            }
            Rigidbody rigidbody = damageable.Root.GetComponent<Rigidbody>();
            if (rigidbody != null) {
                rigidbody.useGravity = false;
            }
        }
        GameObject replacementObject = Instantiate(replacementObjectPrefab, damageable.Root.position, damageable.Root.rotation);
        Damageable replacementDamageable = replacementObject.GetComponent<Damageable>();
        replacementDamageable.SetParentDamageable(damageable);
        replacementObject.name = ReplacementObjectName;
        replacementObject.transform.parent = damageable.Root;
    }

    public override void OnRemoveEffect(Damageable damageable) {

        Transform transmuteObject = damageable.Root.Find(ReplacementObjectName);
        if (transmuteObject) {
            damageable.Root.position = transmuteObject.position;
            Destroy(transmuteObject.gameObject);
        }

        CharacterBehaviour characterBehaviour = damageable.Root.GetComponent<CharacterBehaviour>();
        if (characterBehaviour != null) {
            characterBehaviour.BodyTransform.gameObject.SetActive(true);
            CharacterMoveController characterMove = characterBehaviour.GetComponent<CharacterMoveController>();
            characterMove.enabled = true;
            characterMove.CharacterController.enabled = true;
        } else {
            MeshRenderer meshRenderer = damageable.Root.GetComponent<MeshRenderer>();
            if (meshRenderer != null) {
                meshRenderer.enabled = true;
            }
            Collider collider = damageable.Root.GetComponent<Collider>();
            if (collider != null) {
                collider.enabled = true;
            }
            Rigidbody rigidbody = damageable.Root.GetComponent<Rigidbody>();
            if(rigidbody != null) {
                rigidbody.useGravity = true;
            }
        }
    }
}
