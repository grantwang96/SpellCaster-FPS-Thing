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

            PooledObject pooledObject = ObjectPool.Instance.UsePooledObject(_magicProjectilePrefabId);
            MagicProjectile magicProjectile = pooledObject as MagicProjectile;
            if(magicProjectile == null) {
                Debug.LogError($"Object \"{pooledObject}\" retrieved with ID \"{_magicProjectilePrefabId}\" was not of type MagicProjectile.");
                return;
            }
            InitializeMagicProjectile(magicProjectile, startPosition, direction, caster, spell);

            int power = 1;
            if (ArrayHelper.Contains(spellTiming, SpellTiming.Charge)) {
                power += Mathf.FloorToInt(caster.ActiveSpell.holdTime);
            }

            magicProjectile.InitializeStats(_projectileSpeed, _lifeTime, power); // wip
        }
    }
}
