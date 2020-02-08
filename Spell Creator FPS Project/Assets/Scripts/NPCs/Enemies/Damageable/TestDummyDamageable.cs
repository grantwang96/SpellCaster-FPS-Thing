using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDummyDamageable : Damageable {

    [SerializeField] private int _health;
    public override int Health { get { return _health; } }
    [SerializeField] private int _maxHealth;
    public override int MaxHealth {
        get {
            return _maxHealth;
        }
    }
    [SerializeField] private bool _isDead;
    public override bool IsDead {
        get {
            return _isDead;
        }
    }
    [SerializeField] private Transform _body;
    public override Transform Body => _body;

    private CharacterMoveController _moveController;

	// Use this for initialization
	void Start () {
        _moveController = GetComponent<CharacterMoveController>();
	}

    public override void TakeDamage(int damage, Element element) {
        if(_parentDamageable != null) {
            _parentDamageable.TakeDamage(damage, element);
            return;
        }
        Debug.Log(name + " has received " + damage + " damage!");
        _health -= damage;
    }

    public override void TakeDamage(int damage, Element element, Vector3 velocity) {
        if (_parentDamageable != null) {
            _parentDamageable.TakeDamage(damage, element, velocity);
            return;
        }
        TakeDamage(damage, element);
        AddForce(velocity);
    }

    public override void TakeDamage(int power, Element element, StatusEffect statusEffect) {
        if (_parentDamageable != null) {
            _parentDamageable.TakeDamage(power, element, statusEffect);
            return;
        }
        AddStatusEffect(statusEffect, power);
    }

    public override void TakeDamage(int damage, Element element, Vector3 velocity, StatusEffect statusEffect) {
        if (_parentDamageable != null) {
            _parentDamageable.TakeDamage(damage, element, velocity, statusEffect);
            return;
        }
        TakeDamage(damage, element, velocity);
        AddStatusEffect(statusEffect, damage);
    }

    public override void AddForce(Vector3 velocity, float drag = 0f, bool overrideForce = false, bool allowControl = false) {
        if (overrideForce) {
            _moveController.OverrideForce(velocity, drag, allowControl);
        } else {
            _moveController.AddForce(velocity, drag, allowControl);
        }
    }
}
