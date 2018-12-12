using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the spellbook that is found in world space
/// </summary>
public class SpellBook : MonoBehaviour, IInteractable {

    [SerializeField] private Spell _heldSpell;
    public Spell HeldSpell {
        get { return _heldSpell; }
        set { _heldSpell = value; }
    }

    [SerializeField] private bool _interactable = true;

    // Get Components here
    void Awake() {

    }

	// Use this for initialization
	void Start () {
        Initialize();
	}

    private void Initialize() {
        Spell newSpell = SpellManager.Instance.GenerateRandomSpell();
        _heldSpell = newSpell;
    }

    public void Interact(CharacterBehaviour character) {
        if (!_interactable) { return; }
        ISpellCaster caster = character.GetComponent<ISpellCaster>();
        if(caster != null) {
            _interactable = false;
            caster.PickUpSpell(HeldSpell);
            Destroy(gameObject);
        }
    }

    public void Detect() {
        
    }
}
