using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDoor : MonoBehaviour, IInteractable {

    public bool Interactable { get; private set; }
    public Vector3 InteractableCenter => transform.position;

    [SerializeField] private string _interactableId;
    public string InteractableId => _interactableId;
    [SerializeField] private Door _parentDoor;

    public event InteractEvent OnInteractAttempt;
    public event InteractEvent OnInteractSuccess;
    
    public void InteractPress(CharacterBehaviour character) {
        if (character == PlayerController.Instance) {
            _parentDoor.PlayerInteracted();
        }
    }

    public void InteractHold(CharacterBehaviour character) {

    }

    public void Detect() {

    }
}
