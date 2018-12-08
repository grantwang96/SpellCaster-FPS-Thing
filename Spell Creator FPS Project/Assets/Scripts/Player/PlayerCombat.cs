using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour, ISpellCaster {

    public Transform GunBarrel { get; set; }

    private IDamageable _damageable;
    public IDamageable Damageable {
        get {
            return _damageable;
        }
    }
    private CharacterBehaviour _characterBehaviour;
    public CharacterBehaviour CharacterBehaviour {
        get {
            return _characterBehaviour;
        }
    }
    private SpellsInventory _spellsInventory;
    public SpellsInventory SpellsInventory {
        get {
            return _spellsInventory;
        }
    }

    [SerializeField] private int selectedSpellIndex;
    private Spell SelectedSpell {
        get {
            if(_spellsInventory.SpellsList.Length == 0) { return null; }
            if(selectedSpellIndex >= _spellsInventory.SpellsList.Length) { selectedSpellIndex = 0; }
            return _spellsInventory.SpellsList[selectedSpellIndex];
        }
    }

    void Awake() {
        _damageable = GetComponent<IDamageable>();
        _characterBehaviour = GetComponent<CharacterBehaviour>();
        _spellsInventory = GetComponent<SpellsInventory>();
    }

    void OnEnable() {
        GameplayController.Instance.OnFire1Pressed += OnShootPressed;
        GameplayController.Instance.OnFire1Held += OnShootHeld;
        GameplayController.Instance.OnFire1End += OnShootReleased;
    }

    void OnDisable() {
        GameplayController.Instance.OnFire1Pressed -= OnShootPressed;
        GameplayController.Instance.OnFire1Held -= OnShootHeld;
        GameplayController.Instance.OnFire1End -= OnShootReleased;
    }

    private void OnShootPressed() {
        Spell selectedSpell = SelectedSpell;
        if(GunBarrel != null && SelectedSpell != null) {
            // Attempt to fire currently equipped spell
            selectedSpell.OnStartCastSpell(this);
        }
    }

    private void OnShootHeld() {
        Spell selectedSpell = SelectedSpell;
        if (GunBarrel != null && SelectedSpell != null) {
            // Attempt to fire currently equipped spell
            selectedSpell.OnHoldCastSpell(this);
        }
    }

    private void OnShootReleased() {
        Spell selectedSpell = SelectedSpell;
        if (GunBarrel != null && SelectedSpell != null) {
            // Attempt to fire currently equipped spell
            selectedSpell.OnEndCastSpell(this);
        }
    }
}
