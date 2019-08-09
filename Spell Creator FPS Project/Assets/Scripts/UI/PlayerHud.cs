using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour {

    [SerializeField] private int _health;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _mana;
    [SerializeField] private int _maxMana;

    [SerializeField] private Text _healthTextDisplay;
    [SerializeField] private Text _manaTextDisplay;
    [SerializeField] private Image _spellChargeBar;

    [SerializeField] private PlayerDamageable _playerDamageable;
    [SerializeField] private PlayerCombat _playerCombat;

	// Use this for initialization
	void Start () {
        _health = _playerDamageable.Health;
        _maxHealth = _playerDamageable.MaxHealth;
        _mana = _playerCombat.Mana;
        _maxMana = _playerCombat.MaxMana;
        _playerDamageable.OnHealthChanged += PlayerHealthChanged;
        _playerDamageable.OnMaxHealthChanged += PlayerMaxHealthChanged;
        _playerCombat.OnManaChanged += PlayerManaChanged;
        _playerCombat.OnActiveSpellDataUpdated += OnActiveSpellDataUpdated;

        UpdateHealthDisplay(_health, _maxHealth);
        UpdateManaDisplay(_mana, _maxMana);
	}

    private void OnDestroy() {
        _playerDamageable.OnHealthChanged -= PlayerHealthChanged;
        _playerDamageable.OnMaxHealthChanged -= PlayerMaxHealthChanged;
        _playerCombat.OnManaChanged -= PlayerManaChanged;
        _playerCombat.OnActiveSpellDataUpdated -= OnActiveSpellDataUpdated;
    }

    private void PlayerHealthChanged(int newHealth) {
        _health = newHealth;
        UpdateHealthDisplay(newHealth, _maxHealth);
    }

    private void PlayerMaxHealthChanged(int newMaxHealth) {
        _maxHealth = newMaxHealth;
        UpdateHealthDisplay(_health, newMaxHealth);
    }

    private void UpdateHealthDisplay(int health, int maxHealth) {
        _healthTextDisplay.text = $"{health} / {maxHealth} ";
    }

    private void PlayerManaChanged(int newMana) {
        _mana = newMana;
        UpdateManaDisplay(newMana, _maxMana);
    }

    private void UpdateManaDisplay(int mana, int maxMana) {
        _manaTextDisplay.text = $"{mana} / {maxMana}";
    }

    private void OnActiveSpellDataUpdated() {
        ActiveSpell activeSpell = _playerCombat.ActiveSpell;
        if(activeSpell == null) {
            _spellChargeBar.fillAmount = 0f;
            return;
        }
        _spellChargeBar.fillAmount = activeSpell.holdTime / activeSpell.maxHoldTime;
        UpdateManaDisplay(_mana - activeSpell.totalManaCost, _maxMana);
    }
}
