using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Effect/Spell Spawn")]
public class Effect_SpellSpawn : Effect {

    [SerializeField] private string _magicProjectilePrefabId;
    [SerializeField] private int _minBoltsSpawned;
    [SerializeField] private int _maxBoltsSpawned;
    [SerializeField] private float _projectileLifeTime;
    [SerializeField] private bool _useGravity;
    [SerializeField] private float _force;

    public override void TriggerEffect(Damageable caster, int power, List<Effect> additionalEffects = null) {
        SpawnMagicProjectiles(power, caster.Body.position, additionalEffects);
    }

    public override void TriggerEffect(Damageable caster, int power, Vector3 position, Damageable damageable = null, List<Effect> additionalEffects = null) {
        if(damageable == null) {
            return;
        }
        SpawnMagicProjectiles(power, damageable.Body.position, additionalEffects);
    }

    public override void TriggerEffect(Damageable caster, Vector3 velocity, int power, Vector3 position, Damageable damageable = null, List<Effect> additionalEffects = null) {
        TriggerEffect(caster, power, position, damageable, additionalEffects);
    }

    private bool IsEffectListEmpty(List<Effect> additionalEffects = null) {
        if (additionalEffects == null || additionalEffects.Count == 0) {
            Debug.LogWarning($"[{nameof(Effect_SpellSpawn)}] Attempted to create spell spawn with no additional effects!");
            return true;
        }
        return false;
    }

    private void SpawnMagicProjectiles(int power, Vector3 position, List<Effect> additionalEffects) {
        if (IsEffectListEmpty()) {
            return;
        }
        List<Effect> effects = new List<Effect>(additionalEffects);
        effects.Remove(this);
        int boltCount = Random.Range(_minBoltsSpawned, _maxBoltsSpawned);
        for (int i = 0; i < boltCount; i++) {
            Vector3 velocity = Random.insideUnitSphere.normalized * _force;

            MagicProjectile magicProjectile = GetMagicProjectile();
            magicProjectile.InitializeProjectile(power, _projectileLifeTime, effects.ToArray());
            magicProjectile.InitializePosition(position, velocity);

            magicProjectile.ActivatePooledObject();
            magicProjectile.FireProjectile(_useGravity, velocity);
        }
    }

    protected virtual MagicProjectile GetMagicProjectile() {
        // retrieve pooled object and cast as magic projectile
        PooledObject pooledObject = ObjectPool.Instance.UsePooledObject(_magicProjectilePrefabId);
        MagicProjectile magicProjectile = pooledObject as MagicProjectile;
        if (magicProjectile == null) {
            Debug.LogError($"Object \"{pooledObject}\" retrieved with ID \"{_magicProjectilePrefabId}\" was not of type MagicProjectile.");
            return null;
        }
        return magicProjectile;
    }
}
