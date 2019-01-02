using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDummyDamageable : IDamageable {

    [SerializeField] private int _health;
    public int Health { get { return _health; } }
    
    CharacterMoveController moveController;

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
