using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Casting Method/Spread Shot")]
public class CastingMethod_SpreadShot : CastingMethod_MagicProjectile {
    
    [SerializeField] private int _count;
    [SerializeField] private float _spread;

    public override int ManaCost {
        get {
            return manaCost * _count;
        }
    }

	protected override void CastSpell(ISpellCaster caster, Spell spell) {
        Vector3 startPosition = caster.GunBarrel.position;
        for (int i = 0; i < _count; i++) {
            Vector3 target = Random.insideUnitCircle * _spread;
            Vector3 direction = caster.GunBarrel.forward;
            direction += target;

            PooledObject pooledObject;
            if(!PooledObjectManager.Instance.UsePooledObject(_magicProjectilePrefabId, out pooledObject)) {
                return;
            }
            MagicProjectile magicProjectile = pooledObject as MagicProjectile;
            if(magicProjectile == null) {
                Debug.LogError($"Object \"{pooledObject}\" retrieved with ID \"{_magicProjectilePrefabId}\" was not of type MagicProjectile.");
                return;
            }
            float powerScale = GetTotalPowerScale(caster.ActiveSpell, spell);
            magicProjectile.InitializeProjectile(powerScale, _lifeTime, spell.Effects);
            InitializeMagicProjectile(magicProjectile, startPosition, direction, caster, spell);
            magicProjectile.FireProjectile(false, direction.normalized * _projectileSpeed);
        }
    }
}
