using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Casting Method/Spread Shot")]
public class CastingMethod_SpreadShot : Spell_CastingMethod {

    [SerializeField] private MagicProjectile _magicProjectilePrefab;
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _lifeTime;
    [SerializeField] private int _count;
    [SerializeField] private float _spread;

	protected override void CastSpell(ISpellCaster caster, Spell spell) {
        Vector3 startPosition = caster.GunBarrel.position;
        for (int i = 0; i < _count; i++) {
            Vector3 target = Random.insideUnitCircle * _spread;
            Vector3 forward = caster.GunBarrel.forward;
            forward += target;

            MagicProjectile magicProjectile = Instantiate(_magicProjectilePrefab, startPosition, caster.GunBarrel.rotation);
            magicProjectile.transform.forward = forward;
            magicProjectile.InitializeMagic(caster, spell.Effects);
            magicProjectile.InitializeMagicModifiers(spell.SpellModifiers);
            magicProjectile.InitializePosition(startPosition, forward);

            int power = 1;
            if (ArrayHelper.Contains(spellTiming, SpellTiming.Charge)) {
                power += Mathf.FloorToInt(caster.ActiveSpell.holdTime);
            }

            magicProjectile.InitializeStats(_projectileSpeed, _lifeTime, power); // wip
        }
    }
}
