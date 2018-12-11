using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour, ISpellCaster {

    public int mana { get; private set; }
    public ActiveSpell ActiveSpell { get; private set; }

    public Transform GunBarrel { get; set; }
    public IDamageable Damageable { get; private set; }
    public CharacterBehaviour CharacterBehaviour { get; private set; }
    public SpellsInventory SpellsInventory { get; private set; }

    [SerializeField] private int selectedSpellIndex;
    private Spell SelectedSpell {
        get {
            if(SpellsInventory.SpellsList.Count == 0) { return null; }
            if(selectedSpellIndex >= SpellsInventory.SpellsList.Count) { selectedSpellIndex = 0; }
            return SpellsInventory.SpellsList[selectedSpellIndex];
        }
    }

    void Awake() {
        Damageable = GetComponent<IDamageable>();
        CharacterBehaviour = GetComponent<CharacterBehaviour>();
        SpellsInventory = GetComponent<SpellsInventory>();
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
}
