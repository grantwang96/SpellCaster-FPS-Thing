using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCraftingStation : MonoBehaviour, IInteractable {

    public void Detect() {
        
    }

    public void Interact(CharacterBehaviour character) {
        Debug.Log($"You are interacting with spell craft station: {this.name}");
    }
}
