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

    private CharacterMoveController moveController;

	// Use this for initialization
	void Start () {
        moveController = GetComponent<CharacterMoveController>();
	}

    public override void TakeDamage(int damage) {
        if(parentDamageable != null) {
            parentDamageable.TakeDamage(damage);
            return;
        }
        Debug.Log(name + " has received " + damage + " damage!");
        _health -= damage;
    }

    public override void TakeDamage(int damage, Vector3 velocity) {
        if (parentDamageable != null) {
            parentDamageable.TakeDamage(damage, velocity);
            return;
        }
        TakeDamage(damage);
        AddForce(velocity);
    }

    public override void TakeDamage(int power, StatusEffect statusEffect) {
        if (parentDamageable != null) {
            parentDamageable.TakeDamage(power, statusEffect);
            return;
        }
        AddStatusEffect(statusEffect, power);
    }

    public override void TakeDamage(int damage, Vector3 velocity, StatusEffect statusEffect) {
        if (parentDamageable != null) {
            parentDamageable.TakeDamage(damage, velocity, statusEffect);
            return;
        }
        TakeDamage(damage, velocity);
        AddStatusEffect(statusEffect, damage);
    }

    private void Die() {
        gameObject.SetActive(false);
    }

    public override void AddForce(Vector3 velocity) {
        moveController.AddForce(velocity);
    }
}
