using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageable : IDamageable {

    [SerializeField] private int _health;
    public int Health { get { return _health; } }

    private bool _isDead;
    public bool IsDead { get { return _isDead; } }

    private CharacterMoveController moveController;

    public override void TakeDamage(int damage) {
        if (_isDead) { return; }
        _health -= damage;
        if(_health < 0) { Die(); }
    }

    public override void TakeDamage(int damage, Vector3 velocity) {
        TakeDamage(damage);
        AddForce(velocity);
    }

    public override void TakeDamage(int damage, StatusEffect statusEffect) {
        TakeDamage(damage);
    }

    public override void TakeDamage(int damage, Vector3 velocity, StatusEffect statusEffect) {
        TakeDamage(damage, velocity);
    }

    private void Die() {
        _isDead = true;
    }

    public override void AddForce(Vector3 velocity) {
        moveController.AddForce(velocity);
    }
}
