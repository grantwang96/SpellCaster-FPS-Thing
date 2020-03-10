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
            OnManaChanged?.Invoke(Mana);
        }
    }
    [SerializeField] private int _maxMana;
    public int MaxMana {
        get {
            return _maxMana;
        }
    }
    // Contains data for currently active spell(charge time, power, etc.)
    public ActiveSpell ActiveSpell { get; private set; }
    // how long before spell can be fired again
    private bool _coolingDown = false;

    // mana recovery values
    [SerializeField] private float _manaRechargeDelay;
    [SerializeField] private int _manaRechargeRate; // mana recovered per second
    public delegate void ManaChanged(int newMana);
    public event ManaChanged OnManaChanged;
    public delegate void ActiveSpellDataUpdated();
    public event ActiveSpellDataUpdated OnActiveSpellDataUpdated;
    private Coroutine _manaRegeneration;

    [SerializeField] private Transform _gunBarrel;
    public Transform GunBarrel { get { return _gunBarrel; } }
    public Damageable Damageable { get; private set; }
    public CharacterBehaviour CharacterBehaviour { get; private set; }

    [SerializeField] private List<Spell> _spellsList;
    public List<Spell> SpellsList { get { return _spellsList; } }
    private List<SpellSlotInfo> _spellSlotInfos = new List<SpellSlotInfo>();

    private const int _spellInventoryLimit = GameplayValues.Magic.PlayerLoadoutMaxSize;
    [SerializeField] private int _selectedSpellIndex;
    public int SelectedSpellIndex => _selectedSpellIndex;
    private Spell SelectedSpell {
        get {
            if(_spellsList.Count == 0) { return null; }
            if(_selectedSpellIndex >= _spellsList.Count) { _selectedSpellIndex = 0; }
            return _spellsList[_selectedSpellIndex];
        }
    }

    private bool _active;

    public delegate void SelectedSpellUpdatedDelegate(int index);
    public delegate void SpellInventoryUpdatedDelegate(List<SpellSlotInfo> infos);
    public event SelectedSpellUpdatedDelegate OnSelectedSpellUpdated;
    public event SpellInventoryUpdatedDelegate OnSpellsInventoryUpdated;

    void Awake() {
        // Initialize components
        Damageable = GetComponent<Damageable>();
        CharacterBehaviour = GetComponent<CharacterBehaviour>();
    }

    private void Start() {
        GameManager.GameManagerInstance.CurrentSpellInventory.OnLoadoutDataUpdated += OnLoadoutUpdated;
        InitializeLoadout();
        SubscribeToController();
        OnControllerStateUpdated();
    }

    private void InitializeLoadout() {
        _spellsList = new List<Spell>();
        OnLoadoutUpdated(GameManager.GameManagerInstance.CurrentSpellInventory.CurrentLoadout);
    }

    private void OnLoadoutUpdated(StorableSpell[] loadout) {
        _spellsList.Clear();
        _spellSlotInfos.Clear();
        for(int i = 0; i < loadout.Length; i++) {
            if(loadout[i] == null) {
                continue;
            }
            Spell newSpell = loadout[i].ConvertToSpell();
            if(newSpell == null) {
                Debug.LogError($"[PlayerCombat] Spell derived from loadout index {i} is null!");
                continue;
            }
            _spellsList.Add(newSpell);
            // add to spell slot infos
            AddSpellSlotInfo(newSpell);
        }
        OnSpellsInventoryUpdated?.Invoke(_spellSlotInfos);
    }

    private void SubscribeToController() {
        PlayerController.Instance.MouseScrollEvent += OnMouseScrollEvent;
        PlayerController.Instance.OnFire1Pressed += OnFire1Pressed;
        PlayerController.Instance.OnFire1Held += OnFire1Held;
        PlayerController.Instance.OnFire1Released += OnFire1Released;
        PlayerController.Instance.OnSlotBtnPressed += OnSlotButtonPressed;
        PlayerController.Instance.OnControllerStateUpdated += OnControllerStateUpdated;
        PlayerController.Instance.Damageable.OnDeath += OnPlayerDeath;
    }

    private void UnsubscribeToController() {
        PlayerController.Instance.MouseScrollEvent -= OnMouseScrollEvent;
        PlayerController.Instance.OnFire1Pressed -= OnFire1Pressed;
        PlayerController.Instance.OnFire1Held -= OnFire1Held;
        PlayerController.Instance.OnFire1Released -= OnFire1Released;
        PlayerController.Instance.OnSlotBtnPressed -= OnSlotButtonPressed;
        PlayerController.Instance.OnControllerStateUpdated -= OnControllerStateUpdated;
        PlayerController.Instance.Damageable.OnDeath -= OnPlayerDeath;
    }

    void OnDisable() {
        GameManager.GameManagerInstance.CurrentSpellInventory.OnLoadoutDataUpdated -= OnLoadoutUpdated;
    }

    private void OnControllerStateUpdated() {
        _active = PlayerController.Instance.ControllerState == ControllerState.Gameplay;
    }

    private void OnPlayerDeath(bool isDead, Damageable damageable) {
        UnsubscribeToController();
    }

    private void OnMouseScrollEvent(float mouseDelta) {
        int nextSlot = _selectedSpellIndex - Mathf.RoundToInt(mouseDelta);
        if(nextSlot < 0) {
            nextSlot = _spellsList.Count - 1;
        } else if(nextSlot >= _spellsList.Count) {
            nextSlot = 0;
        }
        OnSlotButtonPressed(nextSlot + 1);
    }

    private void OnFire1Pressed() {
        if (!CanFireSpell()) { return; }
        InitializeActiveSpell();
        if (_mana < ActiveSpell.totalManaCost) {
            return;
        }
        if (_manaRegeneration != null) {
            StopCoroutine(_manaRegeneration);
        }
        // Attempt to fire currently equipped spell
        bool successfullyCast = SelectedSpell.OnStartCastSpell(this);
        if (successfullyCast) {
            FinishedCastSpell(ActiveSpell.baseManaCost);
        }
    }

    private void OnFire1Held() {
        if (!CanFireSpell()) { return; }
        if (ActiveSpell == null) {
            InitializeActiveSpell();
        }
        UpdateActiveSpell();
        if (_mana < ActiveSpell.totalManaCost) {
            return;
        }
        // Attempt to fire currently equipped spell
        bool successfullyCast = SelectedSpell.OnHoldCastSpell(this);
        if (successfullyCast) {
            FinishedCastSpell(ActiveSpell.totalManaCost);
        }
    }

    private void OnFire1Released() {
        if (!CanFireSpell() || ActiveSpell == null) {
            return;
        }
        if(_mana < ActiveSpell.totalManaCost) {
            ActiveSpell = new ActiveSpell();
            return;
        }
        // Attempt to fire currently equipped spell
        bool successfullyCast = SelectedSpell.OnEndCastSpell(this);
        if (successfullyCast) {
            FinishedCastSpell(ActiveSpell.totalManaCost);
        }
        ActiveSpell = null;
        OnActiveSpellDataUpdated?.Invoke();
    }

    private bool CanFireSpell() {
        return _active && GunBarrel != null && SelectedSpell != null && !_coolingDown;
    }

    private void InitializeActiveSpell() {
        ActiveSpell = new ActiveSpell() {
            holdTime = 0f,
            interval = SelectedSpell.IntervalTime,
            maxHoldTime = SelectedSpell.MaxChargeTime,
            baseManaCost = SelectedSpell.ManaCost
        };
        OnActiveSpellDataUpdated?.Invoke();
    }

    private void OnSlotButtonPressed(int number) {
        if(number <= 0 || number > _spellsList.Count) { return; }
        _selectedSpellIndex = number - 1;
        OnSelectedSpellUpdated?.Invoke(_selectedSpellIndex);
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
        OnActiveSpellDataUpdated?.Invoke();
    }

    private int CalculateTotalManaCost(int baseManaCost, float holdTime) {
        return Mathf.Max(Mathf.RoundToInt(baseManaCost * (1f + holdTime)), baseManaCost);
    }

    private void FinishedCastSpell(int lostMana) {
        Mana = Mathf.Max(Mana - lostMana, 0);
        ActiveSpell.holdIntervalTime = 0f;
        ActiveSpell.holdTime = 0f;
        StartCoroutine(CoolDownSpell(ActiveSpell.interval));
        if(_manaRegeneration != null) { StopCoroutine(_manaRegeneration); }
        _manaRegeneration = StartCoroutine(RegenerateMana());
    }

    private IEnumerator CoolDownSpell(float coolDownTime) {
        _coolingDown = true;
        float time = 0f;
        while(time < coolDownTime) {
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        _coolingDown = false;
    }

    private IEnumerator RegenerateMana() {
        yield return new WaitForSeconds(_manaRechargeDelay);
        float time = 0f;
        float waitTime = 1f / _manaRechargeRate;
        while (_mana < _maxMana) {
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

    private void AddSpellSlotInfo(Spell newSpell) {
        Sprite[] effectIcons = new Sprite[newSpell.Effects.Length];
        for (int j = 0; j < effectIcons.Length; j++) {
            effectIcons[j] = newSpell.Effects[j].SmallIcon;
        }
        Sprite[] modifierIcons = new Sprite[newSpell.SpellModifiers.Length];
        for (int j = 0; j < modifierIcons.Length; j++) {
            modifierIcons[j] = newSpell.SpellModifiers[j].SmallIcon;
        }
        _spellSlotInfos.Add(new SpellSlotInfo(newSpell.Name, newSpell.CastingMethod.SmallIcon, effectIcons, modifierIcons));
    }

    /*
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
    */

    public void RecoverMana(int mana) {
        if(_mana + mana > MaxMana) {
            Mana = MaxMana;
            return;
        }
        Mana += mana;
    }

    /*
    private void DropSpell(Spell spell, Vector3 location, Quaternion rotation) {
        int index = _spellsList.FindIndex(x => x == spell);
        if(index > 0) {
            _spellsList.RemoveAt(index);
            _spellSlotInfos.RemoveAt(index);
        }
        SpellManager.Instance.GenerateSpellBook(spell, location, rotation);
    }
    */
}
