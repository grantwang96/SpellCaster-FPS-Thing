using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour, ISpellCaster {

    [SerializeField] private int _mana;
    public int Mana {
        get {
            return _mana;
        }
        private set {
            _mana = value;
            OnManaChanged?.Invoke(_mana);
        }
    }
    [SerializeField] private int _maxMana;
    public int MaxMana {
        get {
            return _maxMana;
        }
    }
    private ActiveSpell _activeSpell;
    public ActiveSpell ActiveSpell { get { return _activeSpell; } }

    [SerializeField] private float _manaRechargeDelay;
    [SerializeField] private int _manaRechargeRate; // mana recovered per second
    public delegate void ManaChanged(int newMana);
    public event ManaChanged OnManaChanged;
    private Coroutine _manaRegeneration;

    [SerializeField] private Transform _gunBarrel;
    public Transform GunBarrel { get { return _gunBarrel; } }
    public Damageable Damageable { get; private set; }
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
        Damageable = GetComponent<Damageable>();
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
        if(GunBarrel == null || SelectedSpell == null) {
            return;
        }
        _activeSpell = new ActiveSpell() {
            holdTime = 0f,
            interval = selectedSpell.IntervalTime,
            maxHoldTime = selectedSpell.MaxChargeTime,
            baseManaCost = selectedSpell.ManaCost
        };
        if (_mana < selectedSpell.ManaCost) {
            Debug.Log("Not enough mana!");
            return;
        }
        if (_manaRegeneration != null) {
            StopCoroutine(_manaRegeneration);
        }
        // Attempt to fire currently equipped spell
        bool successfullyCast = selectedSpell.OnStartCastSpell(this);
        if (successfullyCast) {
            FinishedCastSpell(ActiveSpell.baseManaCost);
        }
    }

    private void OnFire1Held() {
        Spell selectedSpell = SelectedSpell;
        if (GunBarrel == null || SelectedSpell == null) {
            return;
        }
        if(_activeSpell == null) {
            _activeSpell = new ActiveSpell() {
                holdTime = 0f,
                interval = selectedSpell.IntervalTime,
                maxHoldTime = selectedSpell.MaxChargeTime,
                baseManaCost = selectedSpell.ManaCost
            };
        }
        UpdateActiveSpell();
        if (_mana < selectedSpell.ManaCost) {
            return;
        }
        // Attempt to fire currently equipped spell
        bool successfullyCast = selectedSpell.OnHoldCastSpell(this);
        if (successfullyCast) {
            FinishedCastSpell(ActiveSpell.baseManaCost);
        }
    }

    private void OnFire1Released() {
        Spell selectedSpell = SelectedSpell;
        if (GunBarrel == null || SelectedSpell == null) {
            return;
        }
        if(_mana < selectedSpell.ManaCost) {
            _activeSpell = new ActiveSpell();
            return;
        }
        // Attempt to fire currently equipped spell
        bool successfullyCast = selectedSpell.OnEndCastSpell(this);
        if (successfullyCast) {
            FinishedCastSpell(ActiveSpell.totalManaCost);
        }
        _activeSpell = null;
    }

    private void UpdateActiveSpell() {
        if(_mana > _activeSpell.totalManaCost) {
            _activeSpell.holdTime += Time.deltaTime;
        }
        _activeSpell.holdIntervalTime += Time.deltaTime;
        if(ActiveSpell.holdTime > ActiveSpell.maxHoldTime) {
            _activeSpell.holdTime = _activeSpell.maxHoldTime;
        }
        if(ActiveSpell.holdIntervalTime > ActiveSpell.interval) {
            _activeSpell.holdIntervalTime = 0f;
        }

        _activeSpell.totalManaCost = CalculateTotalManaCost(_activeSpell.baseManaCost, _activeSpell.holdTime);
    }

    private int CalculateTotalManaCost(int baseManaCost, float holdTime) {
        return Mathf.RoundToInt(baseManaCost * (1f + holdTime));
    }

    private void FinishedCastSpell(int lostMana) {
        Mana -= lostMana;
        if(_manaRegeneration != null) { StopCoroutine(_manaRegeneration); }
        _manaRegeneration = StartCoroutine(RegenerateMana());
    }

    private IEnumerator RegenerateMana() {
        yield return new WaitForSeconds(_manaRechargeDelay);
        float time = 0f;
        while(_mana < _maxMana) {
            float waitTime = 1f / _manaRechargeRate;
            time += Time.deltaTime;
            if(time >= waitTime) {
                Mana++;
                time = 0f;
            }
            yield return null;
        }
        _manaRegeneration = null;
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
