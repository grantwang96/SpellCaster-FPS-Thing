using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Effect/Hold")]
public class Effect_Hold : Spell_Effect {

    [SerializeField] private float _baseHoldRange;
    [SerializeField] private float _holdPowerModifier;

    // for not raycasting
    [SerializeField] private float _risingDuration;
    [SerializeField] private float _baseDuration;

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

    public override void TriggerEffect(ISpellCaster caster, int power, Spell castedSpell, Collider collider) {
        // calculate distance at which the object is being held
        float distance = Vector3.Distance(caster.GunBarrel.position, collider.transform.position);
        // get position according to where caster is looking
        Vector3 targetPosition = caster.GunBarrel.position + caster.GunBarrel.forward * distance;
        // apply external forces while maintaining its distance from the caster
        Vector3 directionToTargetPosition = targetPosition - collider.transform.position;
        collider.attachedRigidbody?.AddForce(directionToTargetPosition, ForceMode.VelocityChange);
    }

    public override void TriggerEffect(ISpellCaster caster, Vector3 velocity, int power, Spell castedSpell, Vector3 position, Damageable damageable = null) {
        damageable?.StartCoroutine(RisingHold(power, damageable));
    }

    private IEnumerator RisingHold(float duration, Damageable damageable) {
        float time = 0f;
        while(time < _risingDuration) {
            time += Time.deltaTime;
            damageable.AddForce(Vector3.up);
            yield return null;
        }
        time = 0f;
        Vector3 position = damageable.transform.position;
        while(time < duration) {
            Vector3 direction = position - damageable.transform.position;
            damageable.AddForce(direction * _holdPowerModifier);
            time += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator RisingHold(float duration, Rigidbody rigidbody) {
        float time = 0f;
        while (time < _risingDuration) {
            time += Time.deltaTime;
            rigidbody.AddForce(Vector3.up * rigidbody.mass * (1f - (time / _risingDuration)));
            yield return null;
        }
        time = 0f;
        Vector3 position = rigidbody.transform.position;
        while (time < duration) {
            Vector3 direction = position - rigidbody.transform.position;
            rigidbody.AddForce(direction * rigidbody.mass);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
