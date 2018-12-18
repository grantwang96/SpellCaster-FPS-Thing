using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Casting Method/Spread Shot")]
public class CastingMethod_SpreadShot : Spell_CastingMethod {

    [SerializeField] private MagicProjectile _magicProjectilePrefab;
    [SerializeField] private float _speed;
    [SerializeField] private float _lifeTime;
    [SerializeField] private int _count;
    [SerializeField] private float _spread;

	protected override void ApplyEffects(ISpellCaster caster, Spell_Effect[] effects) {
        for(int i = 0; i < _count; i++) {
            Vector3 target = Random.insideUnitCircle * _spread;
            Vector3 forward = caster.GunBarrel.forward;
            forward += target;

            MagicProjectile newMagicProjectile = Instantiate(_magicProjectilePrefab, caster.GunBarrel.position, Quaternion.identity);
            newMagicProjectile.Initialize(caster, effects, caster.GunBarrel.position, forward, _speed, _lifeTime);
        }
    }
}
