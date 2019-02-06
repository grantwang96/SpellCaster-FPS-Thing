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

        UpdateHealthDisplay();
        UpdateManaDisplay();
	}

    private void OnDestroy() {
        _playerDamageable.OnHealthChanged -= PlayerHealthChanged;
        _playerDamageable.OnMaxHealthChanged -= PlayerMaxHealthChanged;
        _playerCombat.OnManaChanged -= PlayerManaChanged;
    }

    private void PlayerHealthChanged(int newHealth) {
        _health = newHealth;
        UpdateHealthDisplay();
    }

    private void PlayerMaxHealthChanged(int newMaxHealth) {
        _maxHealth = newMaxHealth;
        UpdateHealthDisplay();
    }

    private void UpdateHealthDisplay() {
        _healthTextDisplay.text = $"{_health} / {_maxHealth} ";
    }

    private void PlayerManaChanged(int newMana) {
        _mana = newMana;
        UpdateManaDisplay();
    }

    private void UpdateManaDisplay() {
        _manaTextDisplay.text = $"{_mana} / {_maxMana}";
    }
}
