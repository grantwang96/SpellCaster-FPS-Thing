using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class CharacterDamageable : MonoBehaviour, Damageable
{
    [SerializeField] protected int _health;
    public int Health => _health;
    [SerializeField] protected int _maxHealth;
    public int MaxHealth => _maxHealth;
    [SerializeField] protected bool _isDead;
    public bool IsDead => _isDead;
    [SerializeField] protected Transform _root;
    public Transform Root => _root;
    [SerializeField] protected Transform _body;
    public Transform Body => _body;

    protected float _stunThreshold;
    protected float _knockbackThreshold;

    protected Damageable _parentDamageable;
    public Damageable ParentDamageable => _parentDamageable;

    protected List<ActiveStatusEffect> _activeStatusEffects = new List<ActiveStatusEffect>();
    protected CharacterMoveController _moveController;

    public event Action<Vector3, float> OnStun;
    public event Action<Vector3, float> OnKnockback;
    public event Action<int> OnHealthChanged;
    public event Action<int> OnMaxHealthChanged;

    public event Action<DamageData> OnDamaged;
    public event Action<bool, Damageable> OnDeath;

    protected virtual void Awake() {
        _moveController = GetComponent<CharacterMoveController>();
    }

    protected virtual void Update() {
        ProcessActiveStatusEffects();
    }

    public virtual void TakeDamage(Damageable attacker, int damage, Element element = Element.Neutral, Vector3 velocity = new Vector3(), StatusEffect statusEffect = null) {
        if (ParentDamageable != null) {
            ParentDamageable.TakeDamage(attacker, damage, element, velocity, statusEffect);
            return;
        }
        if (statusEffect != null) {
            AddStatusEffect(statusEffect, damage);
        }
        _health -= damage;
        OnHealthChanged?.Invoke(_health);
        if (_health <= 0 && !_isDead) {
            Die();
            return;
        }
        OnDamaged?.Invoke(new DamageData(attacker, damage, element, velocity, statusEffect));
    }

    protected virtual bool TryApplyForce(Vector3 force, int power) {
        if(power >= _knockbackThreshold) {
            OnKnockback?.Invoke(force, power);
            return true;
        } else if(power >= _stunThreshold) {
            OnStun?.Invoke(force, power);
            return true;
        }
        return false;
    }

    public virtual void AddForce(Vector3 force, int power, float drag = 0f, bool overrideForce = false, bool allowControl = false) {
        if(!TryApplyForce(force, power)) {
            return;
        }
        if (overrideForce) {
            _moveController.OverrideForce(force, drag, allowControl);
        } else {
            _moveController.AddForce(force, drag, allowControl);
        }
    }

    public virtual void SetParentDamageable(Damageable parent) {
        _parentDamageable = parent;
    }

    protected virtual void AddStatusEffect(StatusEffect statusEffect, int power) {
        ActiveStatusEffect newActiveStatus =
            new ActiveStatusEffect(power, statusEffect);
        for (int i = 0; i < _activeStatusEffects.Count; i++) {
            StatusEffect currentStatusEffect = _activeStatusEffects[i].StatusEffect;
            if (currentStatusEffect == statusEffect) {
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
            if (activeStatusEffect.Duration <= 0) {
                activeStatusEffect.StatusEffect.OnRemoveEffect(this);
                _activeStatusEffects.Remove(activeStatusEffect);
                continue;
            }
            if (activeStatusEffect.Duration <= activeStatusEffect.LastEffectTime) {
                activeStatusEffect.StatusEffect.ApplyEffect(this, 1);
                activeStatusEffect.LastEffectTime = activeStatusEffect.Duration - activeStatusEffect.StatusEffect.Interval;
            }
        }
    }

    protected virtual void Die() {
        _isDead = true;
        OnDeath?.Invoke(true, this);
    }
}
