using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour, IRaycastInteractable {
    
    public string InteractableId { get; private set; }
    public bool Interactable { get; private set; } = true;
    public Vector3 InteractableCenter => transform.position;

    public event InteractEvent OnInteractAttempt;
    public event InteractEvent OnInteractSuccess;

    public void Detect(CharacterBehaviour character) {

    }

    public void Undetect() {

    }

    public void InteractHold(CharacterBehaviour character) {
        // don't do this
    }

    public void InteractPress(CharacterBehaviour character) {
        OnInteractAttempt?.Invoke();
        if (!Interactable) {
            return;
        }
        Interactable = false;
        OnInteractSuccess?.Invoke();
        gameObject.SetActive(false);
    }
}
