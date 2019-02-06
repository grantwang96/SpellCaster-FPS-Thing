using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Damageable : MonoBehaviour {

    public abstract int Health { get; }
    public abstract int MaxHealth { get; }
    public abstract bool IsDead { get; }

    public delegate void HealthChanged(int health);
    public event HealthChanged OnHealthChanged;
    public event HealthChanged OnMaxHealthChanged;
    public delegate void DeathEvent(bool isDead);
    public event DeathEvent OnDeath;

    public Damageable parentDamageable;
    protected List<ActiveStatusEffect> _activeStatusEffects = new List<ActiveStatusEffect>();

    protected virtual void Update() {
        ProcessActiveStatusEffects();
    }

    public abstract void TakeDamage(int power);
    public abstract void TakeDamage(int power, Vector3 velocity);
    public abstract void TakeDamage(int power, StatusEffect statusEffect);
    public abstract void TakeDamage(int power, Vector3 velocity, StatusEffect statusEffect);

    protected void FireHealthUpdateEvent() {
        OnHealthChanged?.Invoke(Health);
    }
    protected void FireMaxHealthUpdatedEvent() {
        OnMaxHealthChanged?.Invoke(MaxHealth);
    }

    protected virtual void AddStatusEffect(StatusEffect statusEffect, int power) {
        ActiveStatusEffect newActiveStatus =
            new ActiveStatusEffect(power, statusEffect);
        for (int i = 0; i < _activeStatusEffects.Count; i++) {
            StatusEffect currentStatusEffect = _activeStatusEffects[i].StatusEffect;
            if(currentStatusEffect == statusEffect) {
                _activeStatusEffects[i] = newActiveStatus;
                return;
            }
        }
        _activeStatusEffects.Add(newActiveStatus);
        newActiveStatus.StatusEffect.OnAddEffect(this, power);
    }
    protected virtual void ProcessActiveStatusEffects() {
        for (int i = 0; i < _activeStatusEffects.Count; i++) {
            ActiveStatusEffect activeStatusEffect = _activeStatusEffects[i];
            activeStatusEffect.Duration -= Time.deltaTime;
            if(activeStatusEffect.Duration <= 0) {
                activeStatusEffect.StatusEffect.OnRemoveEffect(this);
                _activeStatusEffects.Remove(activeStatusEffect);
                continue;
            }
            if(activeStatusEffect.Duration <= activeStatusEffect.LastEffectTime) {
                activeStatusEffect.StatusEffect.ApplyEffect(this, 1);
                activeStatusEffect.LastEffectTime = activeStatusEffect.Duration - activeStatusEffect.StatusEffect.Interval;
            }
        }
    }

    public abstract void AddForce(Vector3 velocity);
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
