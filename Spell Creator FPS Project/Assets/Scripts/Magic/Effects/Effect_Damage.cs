﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic damage effect
/// </summary>
public class Effect_Damage : Spell_Effect {

    [SerializeField] private int _basePower;
    public int BasePower { get { return _basePower; } }
    [SerializeField] private Element _element;
    public Element Element { get { return _element; } }

    public override void TriggerEffect(IDamageable damageable, ISpellCaster caster) {
        Debug.Log("Applied damage: " + BasePower);
        damageable.TakeDamage(BasePower);
    }
}
