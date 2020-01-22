using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void InteractEvent();

// TODO: Potentially further split these interfaces into granularity

public interface IInteractable {
    string InteractableId { get; }
    bool Interactable { get; }

    Vector3 InteractableCenter { get; }

    event InteractEvent OnInteractAttempt; // called when an interaction is attempted (not necessarily successful)
    event InteractEvent OnInteractSuccess; // successful interaction
}

public interface IRaycastInteractable : IInteractable {
    
    void InteractPress(CharacterBehaviour character);
    void InteractHold(CharacterBehaviour character);
    void Detect(CharacterBehaviour character); // should display message when looking at interactable object
    void Undetect();
}

public interface ITriggerAreaInteractable : IInteractable {

    void EnterInteractArea(CharacterBehaviour character);
    void StayInteractArea(CharacterBehaviour character);
}
