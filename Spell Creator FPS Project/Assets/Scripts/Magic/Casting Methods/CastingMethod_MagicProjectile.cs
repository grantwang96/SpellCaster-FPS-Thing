using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CastingMethod_MagicProjectile : Spell_CastingMethod {

    [SerializeField] protected string _magicProjectilePrefabId;
    [SerializeField] protected float _projectileSpeed;
    [SerializeField] protected float _lifeTime;

    protected virtual MagicProjectile GetMagicProjectile() {
        // retrieve pooled object and cast as magic projectile
        PooledObject pooledObject;
        if(!PooledObjectManager.Instance.UsePooledObject(_magicProjectilePrefabId, out pooledObject)){
            return null;
        }
        MagicProjectile magicProjectile = pooledObject as MagicProjectile;
        if (magicProjectile == null) {
            Debug.LogError($"Object \"{pooledObject}\" retrieved with ID \"{_magicProjectilePrefabId}\" was not of type MagicProjectile.");
            return null;
        }
        return magicProjectile;
    }

    protected virtual void InitializeMagicProjectile(MagicProjectile magicProjectile, Vector3 startPosition, Vector3 direction, ISpellCaster caster, Spell spell) {
        magicProjectile.InitializeMagic(caster, spell);
        magicProjectile.InitializePosition(startPosition, direction);
        magicProjectile.ActivatePooledObject();
    }
}
