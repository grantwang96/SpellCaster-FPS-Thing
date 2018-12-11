using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Damage : Spell_Effect {

    [SerializeField] private int _basePower;
    public int BasePower { get { return _basePower; } }

    public override void TriggerEffect(IDamageable damageable, ISpellCaster caster) {
        Debug.Log("Applied damage: " + BasePower);
        damageable.TakeDamage(BasePower);
    }
}
