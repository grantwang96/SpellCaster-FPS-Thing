using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates an area of effect that applies the rest of the effects upon hit
/// </summary>
[CreateAssetMenu(menuName = "Spell Effect/Explosion")]
public class Effect_Explosion : Effect {

    [SerializeField] private float _radius;
    [SerializeField] private float _force;
    [SerializeField] [Range(0f, 1f)] private float _time; // time it takes for explosion to reach full size

    [SerializeField] private MagicExplosion magicExplosionPrefab;

    public override void TriggerEffect(Damageable caster, int power, List<Effect> effects = null) {
        throw new System.NotImplementedException();
    }

    public override void TriggerEffect(Damageable caster, int power, Vector3 position, Damageable damageable = null, List<Effect> additionalEffects = null) {
        List<Effect> newEffects = additionalEffects == null ? new List<Effect>() : new List<Effect>(additionalEffects);
        newEffects.Remove(this);
        MagicExplosion explosion = Instantiate(magicExplosionPrefab, caster.Body.position, Quaternion.identity);
        explosion.Initialize(newEffects, _radius, _force, _time, power, caster);
    }

    public override void TriggerEffect(Damageable caster, int power, Vector3 position, Collider collider, List<Effect> additionalEffects = null) {
        List<Effect> newEffects = additionalEffects == null ? new List<Effect>() : new List<Effect>(additionalEffects);
        newEffects.Remove(this);
        MagicExplosion explosion = Instantiate(magicExplosionPrefab, position, Quaternion.identity);
        explosion.Initialize(newEffects, _radius, _force, _time, power, caster);
    }

    public override void TriggerEffect(Damageable caster, Vector3 velocity, int power, Vector3 position, Damageable damageable = null, List<Effect> effects = null) {
        List<Effect> newEffects = effects == null ? new List<Effect>() : new List<Effect>(effects);
        newEffects.Remove(this);
        MagicExplosion explosion = Instantiate(magicExplosionPrefab, position, Quaternion.identity);
        explosion.Initialize(newEffects, _radius, _force, _time, power, caster, damageable);
    }
}
