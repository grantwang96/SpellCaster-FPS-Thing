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

    private NPCMoveController moveController;
    private NPCBehaviour npcBehaviour;

    // Use this for initialization
    void Start () {
        moveController = GetComponent<NPCMoveController>();
        npcBehaviour = GetComponent<NPCBehaviour>();

        _health = npcBehaviour.Blueprint.TotalHealth;
        _maxHealth = npcBehaviour.Blueprint.TotalHealth;
    }
	
    public override void TakeDamage(int damage) {
        if (parentDamageable != null) {
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

    protected override void AddStatusEffect(StatusEffect statusEffect, int damage) {
        if(ArrayHelper.Contains(npcBehaviour.Blueprint.StatResistances, statusEffect)) {
            return;
        } else if(ArrayHelper.Contains(npcBehaviour.Blueprint.StatWeaknesses, statusEffect)) {
            damage *= 2;
        }
        base.AddStatusEffect(statusEffect, damage);
    }

    private void Die() {
        gameObject.SetActive(false);
    }

    public override void AddForce(Vector3 velocity) {
        moveController.AddForce(velocity);
    }
}
