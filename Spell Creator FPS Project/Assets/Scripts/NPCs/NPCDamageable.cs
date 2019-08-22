using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDamageable : Damageable {

    [SerializeField] private int _health;
    public override int Health => _health;
    [SerializeField] private int _maxHealth;
    public override int MaxHealth => _maxHealth;
    [SerializeField] private bool _isDead;
    public override bool IsDead => _isDead;
    [SerializeField] private Transform _body;
    public override Transform Body => _body;

    private NPCMoveController _moveController;
    private NPCBehaviour _npcBehaviour;

    // Use this for initialization
    void Start () {
        _moveController = GetComponent<NPCMoveController>();
        _npcBehaviour = GetComponent<NPCBehaviour>();

        _health = _npcBehaviour.Blueprint.TotalHealth;
        _maxHealth = _npcBehaviour.Blueprint.TotalHealth;
    }
	
    public override void TakeDamage(int damage) {
        if (parentDamageable != null) {
            parentDamageable.TakeDamage(damage);
            return;
        }
        // Debug.Log($"{this.name} took {damage} points of damage!");
        _health -= damage;
        if(_health <= 0 && !_isDead) {
            Die();
        }
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

    protected override void AddStatusEffect(StatusEffect statusEffect, int damage) {
        if(ArrayHelper.Contains(_npcBehaviour.Blueprint.StatResistances, statusEffect)) {
            return;
        } else if(ArrayHelper.Contains(_npcBehaviour.Blueprint.StatWeaknesses, statusEffect)) {
            damage *= 2;
        }
        base.AddStatusEffect(statusEffect, damage);
    }

    protected override void Die() {
        _isDead = true;
        base.Die();
    }

    public override void AddForce(Vector3 velocity, float drag = 0f) {
        _moveController.AddForce(velocity, drag);
    }
}
