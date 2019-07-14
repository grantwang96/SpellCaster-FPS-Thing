using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the spellbook that is found in world space
/// THIS CLASS HAS BEEN DEPRECATED. SPELLS CAN ONLY BE ACQUIRED THRU CRAFTING/LOADOUT SYSTEM
/// FUTURE ITERATIONS MAY SEE SPELLBOOK RETURN IF MULTIPLAYER BECAME A THING
/// </summary>
public class SpellBook : PooledObject, IInteractable {

    [SerializeField] private Spell _heldSpell;
    public Spell HeldSpell {
        get { return _heldSpell; }
        set { _heldSpell = value; }
    }

    [SerializeField] private bool _interactable = true;
    public Vector3 InteractableCenter => transform.position;

    public event InteractEvent OnInteractAttempt;
    public event InteractEvent OnInteractSuccess;

    public bool Interactable { get { return _interactable; } }

    public string InteractableId {
        get {
            return "spellbook_DEPRECATED";
        }
    }

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

    /// <summary>
    /// Chest should call this if spawning a new spellbook
    /// </summary>
    /// <param name="id"></param>
    public void InitializeFromChest(string id) {
        Spell newSpell = SpellManager.Instance.GenerateSpellFromChestId(id);
        _heldSpell = newSpell;
    }

    public void Interact(CharacterBehaviour character) {
        if (!_interactable) { return; }
        ISpellCaster caster = character.GetComponent<ISpellCaster>();
        if(caster != null) {
            _interactable = false;
            // caster.PickUpSpell(HeldSpell);
            Destroy(gameObject);
        }
    }

    public void Detect() {
        
    }
}
