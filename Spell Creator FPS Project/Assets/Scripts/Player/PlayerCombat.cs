using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour, ISpellCaster {

    public int mana { get; private set; }
    public ActiveSpell ActiveSpell { get; private set; }

    [SerializeField] private Transform _gunBarrel;
    public Transform GunBarrel { get { return _gunBarrel; } }
    public IDamageable Damageable { get; private set; }
    public CharacterBehaviour CharacterBehaviour { get; private set; }

    [SerializeField] private List<Spell> spellsList;
    public List<Spell> SpellsList { get { return spellsList; } }

    [SerializeField] private int spellInventoryLimit;
    [SerializeField] private int selectedSpellIndex;
    private Spell SelectedSpell {
        get {
            if(spellsList.Count == 0) { return null; }
            if(selectedSpellIndex >= spellsList.Count) { selectedSpellIndex = 0; }
            return spellsList[selectedSpellIndex];
        }
    }

    void Awake() {
        Damageable = GetComponent<IDamageable>();
        CharacterBehaviour = GetComponent<CharacterBehaviour>();
    }

    void OnEnable() {
        GameplayController.Instance.OnFire1Pressed += OnFire1Pressed;
        GameplayController.Instance.OnFire1Held += OnFire1Held;
        GameplayController.Instance.OnFire1End += OnFire1Released;
    }

    void OnDisable() {
        GameplayController.Instance.OnFire1Pressed -= OnFire1Pressed;
        GameplayController.Instance.OnFire1Held -= OnFire1Held;
        GameplayController.Instance.OnFire1End -= OnFire1Released;
    }

    private void OnFire1Pressed() {
        Spell selectedSpell = SelectedSpell;
        if(GunBarrel != null && SelectedSpell != null) {
            // Attempt to fire currently equipped spell
            ActiveSpell = new ActiveSpell() {
                holdTime = 0f,
                interval = selectedSpell.intervalTime,
                maxHoldTime = selectedSpell.maxChargeTime
            };
            selectedSpell.OnStartCastSpell(this);
        }
    }

    private void OnFire1Held() {
        Spell selectedSpell = SelectedSpell;
        if (GunBarrel != null && SelectedSpell != null) {
            // Attempt to fire currently equipped spell
            UpdateActiveSpell();
            selectedSpell.OnHoldCastSpell(this);
        }
    }

    private void UpdateActiveSpell() {
        ActiveSpell activeSpell = ActiveSpell;
        activeSpell.holdTime += Time.deltaTime;
        activeSpell.holdIntervalTime += Time.deltaTime;
        if(activeSpell.holdTime > activeSpell.maxHoldTime) {
            activeSpell.holdTime = activeSpell.maxHoldTime;
        }
        if(activeSpell.holdIntervalTime > activeSpell.interval) {
            activeSpell.holdIntervalTime = 0f;
        }
        ActiveSpell = activeSpell;
    }

    private void OnFire1Released() {
        Spell selectedSpell = SelectedSpell;
        if (GunBarrel != null && SelectedSpell != null) {
            // Attempt to fire currently equipped spell
            selectedSpell.OnEndCastSpell(this);
            ActiveSpell = new ActiveSpell();
        }
    }

    public void PickUpSpell(Spell newSpell) {
        if(spellsList.Count >= spellInventoryLimit) {
            Spell dropSpell = spellsList[selectedSpellIndex];
            spellsList[selectedSpellIndex] = newSpell;
            Drop(dropSpell, transform.position + transform.forward, transform.rotation);
        } else {
            spellsList.Add(newSpell);
        }
    }

    private void Drop(Spell spell, Vector3 location, Quaternion rotation) {
        if (spellsList.Contains(spell)) {
            spellsList.Remove(spell);
        }
        SpellManager.Instance.GenerateSpellBook(spell, location, rotation);
    }
}
