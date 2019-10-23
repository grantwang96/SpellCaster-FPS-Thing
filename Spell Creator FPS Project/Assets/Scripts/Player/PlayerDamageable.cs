using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageable : Damageable {

    [SerializeField] private int _health;
    public override int Health { get { return _health; } }
    [SerializeField] private int _maxHealth;
    public override int MaxHealth { get { return _maxHealth; } }
    [SerializeField] private Transform _body;
    public override Transform Body => _body;

    private bool _isDead;
    public override bool IsDead { get { return _isDead; } }

    private CharacterMoveController _moveController;

    private void Awake() {
        _moveController = GetComponent<CharacterMoveController>();
    }

    public override void TakeDamage(int damage, Element element) {
        if (_isDead) { return; }
        _health -= damage;
        if(_health > MaxHealth) { _health = MaxHealth; }
        if(_health < 0) { Die(); }
        FireHealthUpdateEvent();
    }

    public override void TakeDamage(int damage, Element element, Vector3 velocity) {
        TakeDamage(damage, element);
        AddForce(velocity);
    }

    public override void TakeDamage(int damage, Element element, StatusEffect statusEffect) {
        TakeDamage(damage, element);
        AddStatusEffect(statusEffect, damage);
    }

    public override void TakeDamage(int damage, Element element, Vector3 velocity, StatusEffect statusEffect) {
        TakeDamage(damage, element, velocity);
        AddStatusEffect(statusEffect, damage);
    }

    protected override void Die() {
        _health = 0;
        _isDead = true;
        base.Die();
    }

    public override void AddForce(Vector3 velocity, float drag = 0f) {
        _moveController.AddForce(velocity, drag);
    }
}
