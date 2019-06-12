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
    public ActiveSpell ActiveSpell { get; private set; }


    [SerializeField] private float _manaRechargeDelay;
    [SerializeField] private int _manaRechargeRate; // mana recovered per second
    public delegate void ManaChanged(int newMana);
    public event ManaChanged OnManaChanged;
    private Coroutine _manaRegeneration;

    [SerializeField] private Transform _gunBarrel;
    public Transform GunBarrel { get { return _gunBarrel; } }
    public Damageable Damageable { get; private set; }
    public CharacterBehaviour CharacterBehaviour { get; private set; }

    [SerializeField] private List<Spell> _spellsList;
    public List<Spell> SpellsList { get { return _spellsList; } }
    private List<SpellSlotInfo> _spellSlotInfos = new List<SpellSlotInfo>();

    private const int _spellInventoryLimit = 3;
    public int SpellInventoryLimit => _spellInventoryLimit;
    [SerializeField] private int _selectedSpellIndex;
    private Spell SelectedSpell {
        get {
            if(_spellsList.Count == 0) { return null; }
            if(_selectedSpellIndex >= _spellsList.Count) { _selectedSpellIndex = 0; }
            return _spellsList[_selectedSpellIndex];
        }
    }

    public delegate void SelectedSpellUpdatedDelegate(int index);
    public delegate void SpellInventoryUpdatedDelegate(List<SpellSlotInfo> infos);
    public event SelectedSpellUpdatedDelegate OnSelectedSpellUpdated;
    public event SpellInventoryUpdatedDelegate OnSpellsInventoryUpdated;

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
        ActiveSpell = new ActiveSpell() {
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
        if(ActiveSpell == null) {
            ActiveSpell = new ActiveSpell() {
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
            ActiveSpell = new ActiveSpell();
            return;
        }
        // Attempt to fire currently equipped spell
        bool successfullyCast = selectedSpell.OnEndCastSpell(this);
        if (successfullyCast) {
            FinishedCastSpell(ActiveSpell.totalManaCost);
        }
        ActiveSpell = null;
    }

    private void UpdateActiveSpell() {
        if(_mana > ActiveSpell.totalManaCost) {
            ActiveSpell.holdTime += Time.deltaTime;
        }
        ActiveSpell.holdIntervalTime += Time.deltaTime;
        if(ActiveSpell.holdTime > ActiveSpell.maxHoldTime) {
            ActiveSpell.holdTime = ActiveSpell.maxHoldTime;
        }
        if(ActiveSpell.holdIntervalTime > ActiveSpell.interval) {
            ActiveSpell.holdIntervalTime = 0f;
        }

        ActiveSpell.totalManaCost = CalculateTotalManaCost(ActiveSpell.baseManaCost, ActiveSpell.holdTime);
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
        Mana = _maxMana;
    }


    public void PickUpSpell(Spell newSpell) {
        if(_spellsList.Count >= _spellInventoryLimit) {
            Spell dropSpell = _spellsList[_selectedSpellIndex];
            _spellsList[_selectedSpellIndex] = newSpell;
            DropSpell(dropSpell, transform.position + transform.forward, transform.rotation);
        } else {
            _spellsList.Add(newSpell);
        }
        // add to spell slot infos
        Sprite[] effectIcons = new Sprite[newSpell.Effects.Length];
        for(int i = 0; i < effectIcons.Length; i++) {
            effectIcons[i] = newSpell.Effects[i].Icon;
        }
        Sprite[] modifierIcons = new Sprite[newSpell.SpellModifiers.Length];
        for(int i = 0; i < modifierIcons.Length; i++) {
            modifierIcons[i] = newSpell.SpellModifiers[i].Icon;
        }
        _spellSlotInfos.Add(new SpellSlotInfo(newSpell.Name, newSpell.CastingMethod.Icon, effectIcons, modifierIcons));
        OnSpellsInventoryUpdated?.Invoke(_spellSlotInfos);
    }

    public void RecoverMana(int mana) {
        if(_mana + mana > MaxMana) {
            Mana = MaxMana;
            return;
        }
        Mana += mana;
    }

    private void DropSpell(Spell spell, Vector3 location, Quaternion rotation) {
        int index = _spellsList.FindIndex(x => x == spell);
        if(index > 0) {
            _spellsList.RemoveAt(index);
            _spellSlotInfos.RemoveAt(index);
        }
        SpellManager.Instance.GenerateSpellBook(spell, location, rotation);
    }
}
