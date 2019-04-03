using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Effect/Add Status Effect")]
public class Effect_AddStatusEffect : Spell_Effect {

    [SerializeField] private StatusEffect _statusEffect;

    public override void TriggerEffect(ISpellCaster caster, int power, Spell castedSpell) {
        caster.Damageable.TakeDamage(power, _statusEffect);
    }

    public override void TriggerEffect(ISpellCaster caster, int power, Spell castedSpell, Damageable damageable = null) {
        if (caster.Damageable == damageable || damageable == null) { return; }
        damageable.TakeDamage(power, _statusEffect);
    }

    public override void TriggerEffect(ISpellCaster caster, Vector3 velocity, int power, Spell castedSpell, Vector3 position, Damageable damageable = null) {
        if (caster.Damageable == damageable || damageable == null) { return; }
        damageable.TakeDamage(power, _statusEffect);
    }
}
