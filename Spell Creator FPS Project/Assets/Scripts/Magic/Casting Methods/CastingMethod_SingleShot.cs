using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Spell Casting Method/Single Shot")]
public class CastingMethod_SingleShot : CastingMethod_MagicProjectile {

    protected override void CastSpell(ISpellCaster caster, Spell spell) {
        Vector3 startPosition = caster.GunBarrel.position;
        Vector3 direction = caster.GunBarrel.forward;

        MagicProjectile magicProjectile = GetMagicProjectile();
        if(magicProjectile == null) { return; }
        
        // initialize magic projectile
        InitializeMagicProjectile(magicProjectile, startPosition, direction, caster, spell);

        int power = 1;
        if(ArrayHelper.Contains(spellTiming, SpellTiming.Charge)) {
            power += Mathf.FloorToInt(caster.ActiveSpell.holdTime);
        }

        magicProjectile.InitializeStats(_projectileSpeed, _lifeTime, power); // wip: Object pooling not implemented yet
    }
}
