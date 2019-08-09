using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void InteractEvent();

public interface IInteractable {

    string InteractableId { get; }
    bool Interactable { get; }
    Vector3 InteractableCenter { get; }

    event InteractEvent OnInteractAttempt; // called when an interaction is attempted (not necessarily successful)
    event InteractEvent OnInteractSuccess; // successful interaction

    void InteractPress(CharacterBehaviour character);
    void InteractHold(CharacterBehaviour character);
    void Detect(); // should display message when looking at interactable object
}
