using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Spell Casting Method/Single Shot")]
public class CastingMethod_SingleShot : Spell_CastingMethod {

    [SerializeField] private MagicProjectile _magicProjectilePrefab;
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _lifeTime;

    protected override void CastSpell(ISpellCaster caster, Spell spell) {
        Vector3 startPosition = caster.GunBarrel.position;
        Vector3 direction = caster.GunBarrel.forward;
        MagicProjectile magicProjectile = Instantiate(_magicProjectilePrefab, startPosition, caster.GunBarrel.rotation);
        magicProjectile.InitializeMagic(caster, spell.Effects);
        magicProjectile.InitializeMagicModifiers(spell.SpellModifiers);
        magicProjectile.InitializePosition(startPosition, direction);

        int power = 1;
        if(ArrayHelper.Contains(spellTiming, SpellTiming.Charge)) {
            power += Mathf.FloorToInt(caster.ActiveSpell.holdTime);
        }

        magicProjectile.InitializeStats(_projectileSpeed, _lifeTime, power); // wip
    }
}
