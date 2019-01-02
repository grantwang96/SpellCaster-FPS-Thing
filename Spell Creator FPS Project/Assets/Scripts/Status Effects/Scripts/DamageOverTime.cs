﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effect/Damage Over Time")]
public class DamageOverTime : StatusEffect {

    [SerializeField] private int _baseDamage = 1;

    public override void OnAddEffect(IDamageable damageable, int power) {
        ApplyEffect(damageable, power);
    }

    public override void ApplyEffect(IDamageable damageable, int power) {
        int totalPower = _baseDamage * power;
        damageable.TakeDamage(totalPower);
    }
}