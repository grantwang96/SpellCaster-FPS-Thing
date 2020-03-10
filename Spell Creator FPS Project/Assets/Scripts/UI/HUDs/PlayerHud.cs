using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour {

    [SerializeField] private int _health;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _mana;
    [SerializeField] private int _maxMana;

    [SerializeField] private UIProgressBar _healthDisplay;
    [SerializeField] private UIProgressBar _manaDisplay;
    [SerializeField] private UIProgressBar _spellChargeBar;

    [SerializeField] private PlayerDamageable _playerDamageable;
    [SerializeField] private PlayerCombat _playerCombat;

    public static PlayerHud Instance { get; private set; }
    public bool Enabled { get; private set; }

    private void Awake() {
        Instance = this;
    }

    public void SetEnabled(bool enabled) {
        Enabled = enabled;
        gameObject.SetActive(enabled);
        UnsubscribeToEvents();
        if (Enabled) {
            SubscribeToEvents();
        }
    }

    // Use this for initialization
    void Start () {
        _health = _playerDamageable.Health;
        _maxHealth = _playerDamageable.MaxHealth;
        PlayerMaxHealthChanged(_maxHealth);
        _mana = _playerCombat.Mana;
        _maxMana = _playerCombat.MaxMana;
        SubscribeToEvents();
        UpdateHealthDisplay(_health, _maxHealth);
        UpdateManaDisplay(_mana, _maxMana);
        OnActiveSpellDataUpdated();
	}

    private void SubscribeToEvents() {
        _playerDamageable.OnHealthChanged += PlayerHealthChanged;
        _playerDamageable.OnMaxHealthChanged += PlayerMaxHealthChanged;
        _playerCombat.OnManaChanged += PlayerManaChanged;
        _playerCombat.OnActiveSpellDataUpdated += OnActiveSpellDataUpdated;
    }

    private void UnsubscribeToEvents() {
        _playerDamageable.OnHealthChanged -= PlayerHealthChanged;
        _playerDamageable.OnMaxHealthChanged -= PlayerMaxHealthChanged;
        _playerCombat.OnManaChanged -= PlayerManaChanged;
        _playerCombat.OnActiveSpellDataUpdated -= OnActiveSpellDataUpdated;
    }

    private void OnDestroy() {
        UnsubscribeToEvents();
    }

    private void PlayerHealthChanged(int newHealth) {
        _health = newHealth;
        UpdateHealthDisplay(newHealth, _maxHealth);
    }

    private void PlayerMaxHealthChanged(int newMaxHealth) {
        _maxHealth = newMaxHealth;
        _healthDisplay.SetBarSize(new Vector2(newMaxHealth * 2f, 25f)); // hack man wonderland
        UpdateHealthDisplay(_health, newMaxHealth);
    }

    private void UpdateHealthDisplay(int health, int maxHealth) {
        _healthDisplay.UpdateFill((float)health / maxHealth);
    }

    private void PlayerManaChanged(int newMana) {
        _mana = newMana;
        UpdateManaDisplay(_mana, _maxMana);
    }

    private void UpdateManaDisplay(int mana, int maxMana) {
        _manaDisplay.UpdateFill((float)mana / maxMana);
    }

    private void OnActiveSpellDataUpdated() {
        ActiveSpell activeSpell = _playerCombat.ActiveSpell;
        if(activeSpell == null) {
            _spellChargeBar.UpdateFill(0f);
            return;
        }
        _spellChargeBar.UpdateFill(activeSpell.holdTime / activeSpell.maxHoldTime);
    }
}
