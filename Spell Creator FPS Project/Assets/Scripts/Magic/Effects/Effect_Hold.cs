using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Effect/Hold")]
public class Effect_Hold : Spell_Effect {

    [SerializeField] private float _holdPowerModifier; // hack af

    public override void TriggerEffect(ISpellCaster caster, int power, Spell castedSpell) {
        // Can't really hold yourself, can you? Can you?
    }

    public override void TriggerEffect(ISpellCaster caster, int power, Spell castedSpell, Damageable damageable = null) {
        if(damageable == null) {
            return;
        }
        Debug.Log($"Holding {damageable.name}...");
        // calculate distance at which the object is being held
        float distance = Vector3.Distance(caster.GunBarrel.position, damageable.Body.position);
        // get position according to where caster is looking
        Vector3 targetPosition = caster.GunBarrel.position + caster.GunBarrel.forward * distance;
        // apply external forces while maintaining its distance from the caster
        Vector3 directionToTargetPosition = targetPosition - damageable.Body.position;
        damageable.AddForce(directionToTargetPosition * _holdPowerModifier);
    }

    public override void TriggerEffect(ISpellCaster caster, Vector3 velocity, int power, Spell castedSpell, Vector3 position, Damageable damageable = null) {
        
    }
}
