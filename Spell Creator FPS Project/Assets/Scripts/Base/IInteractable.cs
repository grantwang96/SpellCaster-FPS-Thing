using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable {

    void Interact(CharacterBehaviour character);
    void Detect(); // should display message when looking at interactable object
}
