﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
/// <summary>
/// Class that handles "vision/hearing"
/// </summary>
public class DetectionRadius : MonoBehaviour {

    [SerializeField] private NPCBehaviour myBehaviour;
    private SphereCollider _collider;

    private void Start() {
        _collider = GetComponent<SphereCollider>();
        _collider.radius = myBehaviour.Blueprint.VisionRange;
    }

    private void OnTriggerEnter(Collider other) {
        // check if collider has a character behaviour
        CharacterBehaviour otherCharBehaviour = other.GetComponent<CharacterBehaviour>();

        // if so, and it's not us, add to list of "known" characters
        if (otherCharBehaviour && otherCharBehaviour != myBehaviour) {
            myBehaviour.RegisterToKnownCharacters(otherCharBehaviour);
        }
    }

    private void OnTriggerExit(Collider other) {
        // check if collider has a character behaviour
        CharacterBehaviour otherCharBehaviour = other.GetComponent<CharacterBehaviour>();

        // if so, and it's not us, remove from list of "known" characters
        if (otherCharBehaviour && otherCharBehaviour != myBehaviour) {
            myBehaviour.DeregisterFromKnownCharacters(otherCharBehaviour);
        }
    }
}
