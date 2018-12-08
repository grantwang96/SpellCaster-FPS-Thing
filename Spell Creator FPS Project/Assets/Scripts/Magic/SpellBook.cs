using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the spellbook that is found in world space
/// </summary>
public class SpellBook : MonoBehaviour, IInteractable {

    [SerializeField] private Spell _heldSpell;

    // Get Components here
    void Awake() {

    }

	// Use this for initialization
	void Start () {
        Initialize();
	}

    private void Initialize() {

    }

    public void Interact() {

    }

    public void Drop() {

    }
}
