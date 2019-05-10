using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Instant spherecast(?) at wherever the player is pointing
/// </summary>
[CreateAssetMenu(menuName = "Spell Casting Method/Raycast")]
public class CastingMethod_Raycast : Spell_CastingMethod {

    [SerializeField] private float _radius;
    [SerializeField] private float _maxEffectiveRange;
    [SerializeField] private LayerMask _effectiveLayerMask;

    protected override void CastSpell(ISpellCaster caster, Spell spell) {
        RaycastHit hit;
        if (Physics.SphereCast(caster.GunBarrel.position, _radius, caster.GunBarrel.forward, out hit, _maxEffectiveRange, _effectiveLayerMask, QueryTriggerInteraction.Ignore)) {
            List<Effect> effects = new List<Effect>(spell.Effects);
            Damageable damageable = hit.transform.GetComponent<Damageable>();
            if (damageable != null) {
                for (int i = 0; i < spell.Effects.Length; i++) {
                    spell.Effects[i].TriggerEffect(caster.Damageable, spell.Power, damageable, effects);
                }
            } else {
                for (int i = 0; i < spell.Effects.Length; i++) {
                    spell.Effects[i].TriggerEffect(caster.Damageable, spell.Power, hit.collider, effects);
                }
            }
        }
    }
}