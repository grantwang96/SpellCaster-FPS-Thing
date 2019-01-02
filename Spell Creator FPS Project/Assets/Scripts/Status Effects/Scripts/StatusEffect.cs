using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect : ScriptableObject {

    [SerializeField] private float _baseDuration;
    public float BaseDuration { get { return _baseDuration; } }
    [SerializeField] private float _interval;
    public float Interval { get { return _interval; } }

    public virtual void OnAddEffect(IDamageable damageable, int power) { }
    public virtual void ApplyEffect(IDamageable damageable, int power) { }
    public virtual void OnRemoveEffect(IDamageable damageable) { }
}
