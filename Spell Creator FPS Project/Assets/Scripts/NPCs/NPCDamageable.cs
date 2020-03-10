using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDamageable : CharacterDamageable {

    private NPCBehaviour _npcBehaviour;
    private NPCVision _npcVision;

    protected override void Awake() {
        base.Awake();
        _npcBehaviour = GetComponent<NPCBehaviour>();
        _npcVision = GetComponent<NPCVision>();

        _health = _npcBehaviour.Blueprint.TotalHealth;
        _maxHealth = _npcBehaviour.Blueprint.TotalHealth;
        _stunThreshold = _npcBehaviour.Blueprint.StunThreshold;
        _knockbackThreshold = _npcBehaviour.Blueprint.KnockbackThreshold;

        _npcBehaviour.OnCharacterSpawned += OnCharacterSpawned;
    }

    protected void OnCharacterSpawned() {
        _health = _maxHealth;
        _isDead = false;
    }

    public override void TakeDamage(Damageable attacker, int damage, Element element = Element.Neutral, Vector3 velocity = default, StatusEffect statusEffect = null) {
        base.TakeDamage(attacker, damage, element, velocity, statusEffect);
        Debug.Log("Took damage from: " + attacker);
    }

    protected override void AddStatusEffect(StatusEffect statusEffect, int damage) {
        if(ArrayHelper.Contains(_npcBehaviour.Blueprint.StatResistances, statusEffect)) {
            return;
        } else if(ArrayHelper.Contains(_npcBehaviour.Blueprint.StatWeaknesses, statusEffect)) {
            damage *= 2;
        }
        base.AddStatusEffect(statusEffect, damage);
    }
}
