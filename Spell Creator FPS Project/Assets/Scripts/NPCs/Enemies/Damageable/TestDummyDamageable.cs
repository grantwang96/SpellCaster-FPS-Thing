﻿using System.Collections;
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
        Debug.Log(name + " has received " + damage + " damage!");
        _health -= damage;
    }

    public override void TakeDamage(int damage, Vector3 velocity) {
        TakeDamage(damage);
        AddForce(velocity);
    }

    public override void TakeDamage(int power, StatusEffect statusEffect) {
        AddStatusEffect(statusEffect, power);
    }

    public override void TakeDamage(int damage, Vector3 velocity, StatusEffect statusEffect) {
        TakeDamage(damage, velocity);
    }

    private void Die() {
        gameObject.SetActive(false);
    }

    public override void AddForce(Vector3 velocity) {
        moveController.AddForce(velocity);
    }
}
