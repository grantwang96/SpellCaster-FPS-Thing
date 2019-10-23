using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Spell Casting Method/Single Shot")]
public class CastingMethod_SingleShot : CastingMethod_MagicProjectile {

    protected override void CastSpell(ISpellCaster caster, Spell spell) {
        Vector3 startPosition = caster.GunBarrel.position;
        Vector3 direction = caster.GunBarrel.forward;

        MagicProjectile magicProjectile = GetMagicProjectile();
        if (magicProjectile == null) { return; }

        int power = GetPower(spell.Power, caster.ActiveSpell);
        // initialize magic projectile
        magicProjectile.InitializeProjectile(power, _lifeTime, spell.Effects);
        InitializeMagicProjectile(magicProjectile, startPosition, direction, caster, spell);
        magicProjectile.FireProjectile(false, direction.normalized * _projectileSpeed);
    }
}
