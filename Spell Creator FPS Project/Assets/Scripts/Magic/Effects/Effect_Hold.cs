using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * THIS WHOLE EFFECT NEEDS TO BE REFACTORED SOMEHOW ANYWAYS
 * 
 * 
 */

[CreateAssetMenu(menuName = "Spell Effect/Hold")]
public class Effect_Hold : Effect {

    [SerializeField] private float _baseHoldRange;
    [SerializeField] private float _holdPowerModifier; // hack af

    // for not raycasting
    [SerializeField] private float _risingDuration;
    [SerializeField] private float _baseDuration;

    public override void TriggerEffect(Damageable caster, float powerScale, List<Effect> effects = null) {
        // Can't really hold yourself, can you? Can you?
        // maybe you can?...Allows you to float and move?
        Vector3 velocity = Vector3.up;
        caster.AddForce(velocity * _basePower, 999, _holdPowerModifier / powerScale, true, true);
    }

    public override void TriggerEffect(Damageable caster, float powerScale, Vector3 position, Damageable damageable = null, List<Effect> effects = null) {
        if(damageable == null) {
            return;
        }
        if(caster == damageable) {
            TriggerEffect(caster, powerScale, effects);
            return;
        }
        // calculate distance at which the object is being held
        float distance = Vector3.Distance(caster.Root.position, damageable.Body.position);
        // get position according to where caster is looking
        Vector3 targetPosition = caster.Root.position + caster.Root.forward * distance;
        // apply external forces while maintaining its distance from the caster
        Vector3 directionToTargetPosition = targetPosition - damageable.Body.position;
        damageable.AddForce(directionToTargetPosition * _holdPowerModifier, 99999, 0f, true, false);
    }

    public override void TriggerEffect(Damageable caster, float powerScale, Vector3 position, Collider collider, List<Effect> effects = null) {
        // calculate distance at which the object is being held
        float distance = Vector3.Distance(caster.Root.position, collider.transform.position);
        // get position according to where caster is looking
        Vector3 targetPosition = caster.Root.position + caster.Root.forward * distance;
        // apply external forces while maintaining its distance from the caster
        Vector3 directionToTargetPosition = targetPosition - collider.transform.position;
        collider.attachedRigidbody?.AddForce(directionToTargetPosition, ForceMode.VelocityChange);
    }

    public override void TriggerEffect(Damageable caster, Vector3 velocity, float powerScale, Vector3 position, Damageable damageable = null, List<Effect> effects = null) {
        if(damageable == null) {
            return;
        }
        CoroutineGod.Instance.StartCoroutine(RisingHold(GetTotalPower(powerScale), damageable));
    }

    private IEnumerator RisingHold(float duration, Damageable damageable) {
        float time = 0f;
        while (time < _risingDuration) {
            time += Time.deltaTime;
            damageable.AddForce(Vector3.up, _basePower);
            yield return null;
        }
        time = 0f;
        Vector3 position = damageable.Root.position;
        while (time < duration) {
            Vector3 direction = position - damageable.Root.position;
            damageable.AddForce(direction * _holdPowerModifier, 9999, 0f, true, false);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
