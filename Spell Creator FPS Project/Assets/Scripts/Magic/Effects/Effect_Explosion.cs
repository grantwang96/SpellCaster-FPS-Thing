using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates an area of effect that applies the rest of the effects upon hit
/// </summary>
[CreateAssetMenu(menuName = "Spell Effect/Explosion")]
public class Effect_Explosion : Spell_Effect {

    [SerializeField] private float _radius;
    [SerializeField] private float _force;
    [SerializeField] [Range(0f, 1f)] private float _time; // time it takes for explosion to reach full size

    [SerializeField] private MagicExplosion magicExplosionPrefab;

    public override void TriggerEffect(ISpellCaster caster, int power, Spell castedSpell) {
        throw new System.NotImplementedException();
    }

    public override void TriggerEffect(ISpellCaster caster, int power, Spell castedSpell, Damageable damageable = null) {
        throw new System.NotImplementedException();
    }

    public override void TriggerEffect(ISpellCaster caster, Vector3 velocity, int power, Spell castedSpell, Vector3 position, Damageable damageable = null) {
        List<Spell_Effect> effects = new List<Spell_Effect>(castedSpell.Effects);
        effects.Remove(this);
        Spell newCastedSpell = new Spell(castedSpell.CastingMethod, effects.ToArray(), castedSpell.SpellModifiers);
        MagicExplosion explosion = Instantiate(magicExplosionPrefab, position, Quaternion.identity);
        explosion.Initialize(newCastedSpell, _radius, _force, _time, castedSpell.Power, caster, damageable);
    }
}
