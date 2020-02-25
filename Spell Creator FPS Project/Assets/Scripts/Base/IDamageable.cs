using System.Collections.Generic;
using UnityEngine;
using System;

public interface Damageable {

    int Health { get; }
    int MaxHealth { get; }
    bool IsDead { get; }
    Transform Root { get; }
    Transform Body { get; } // center of the Damageable object

    event Action<Vector3, float> OnStun;
    event Action<Vector3, float> OnKnockback;
    event Action<int> OnHealthChanged;
    event Action<int> OnMaxHealthChanged;
    
    event Action<DamageData> OnDamaged;
    event Action<bool, Damageable> OnDeath;

    Damageable ParentDamageable { get; }
    
    void TakeDamage(
        Damageable attacker,
        int power,
        Element element = Element.Neutral,
        Vector3 velocity = new Vector3(),
        StatusEffect statusEffect = null
    );

    void AddForce(Vector3 direction, int power, float drag = 0f, bool overrideForce = false, bool allowControl = false);
    void SetParentDamageable(Damageable parent);
}

[System.Serializable]
public class ActiveStatusEffect {

    public ActiveStatusEffect(int power, StatusEffect statusEffect) {
        Power = power;
        Duration = statusEffect.BaseDuration * power;
        StatusEffect = statusEffect;
        LastEffectTime = Duration - StatusEffect.Interval;
    }

    public int Power { get; private set; }
    public float Duration;
    public float LastEffectTime;
    public StatusEffect StatusEffect { get; private set; }
}

public class DamageData {
    public readonly Damageable Attacker;
    public readonly int Damage;
    public readonly Vector3 Direction;
    public readonly Element Element;
    public readonly StatusEffect StatusEffect;

    public DamageData(Damageable attacker, int damage, Element element, Vector3 direction, StatusEffect statusEffect) {
        Attacker = attacker;
        Damage = damage;
        Direction = direction;
        Element = element;
        StatusEffect = statusEffect;
    }
}
