using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Effect/Add Status Effect")]
public class Effect_AddStatusEffect : Spell_Effect {

    [SerializeField] private StatusEffect _statusEffect;

    public override void TriggerEffect(ISpellCaster caster, int power) {
        caster.Damageable.TakeDamage(power, _statusEffect);
    }

    public override void TriggerEffect(IDamageable damageable, ISpellCaster caster, int power) {
        damageable.TakeDamage(power, _statusEffect);
    }

    public override void TriggerEffect(IDamageable damageable, ISpellCaster caster, Vector3 velocity, int power) {
        damageable.TakeDamage(power, _statusEffect);
    }
}
