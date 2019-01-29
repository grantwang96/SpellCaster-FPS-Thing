using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Effect/TestDummy")]
public class Effect_Dummy : Spell_Effect {

    [SerializeField] private float _upwardForce;

    public override void TriggerEffect(ISpellCaster caster, int power) {
        Debug.Log(caster + " performed spell!");
    }

    public override void TriggerEffect(ISpellCaster caster, int power, IDamageable damageable = null) {
        if (caster.Damageable == damageable || damageable == null) { return; }
        Debug.Log(caster + " performed spell on " + damageable);
    }

    public override void TriggerEffect(ISpellCaster caster, Vector3 velocity, int power, IDamageable damageable = null) {
        if (caster.Damageable == damageable || damageable == null) { return; }
        Debug.Log(caster + " performed spell on " + damageable);
        velocity.y += _upwardForce;
        damageable.AddForce(velocity);
    }
}
