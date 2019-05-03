using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackState : AttackState {

    [SerializeField] private BrainState _targetInRangeState;
    [SerializeField] private BrainState _targetOutOfRangeState;
    [SerializeField] private BrainState _targetMeleeRangeState;

    // pool of projectiles you can fire from
    [SerializeField] private Projectile[] _projectilePool;
    [SerializeField] private float _spawnProjectileTime;
    [SerializeField] private float _fireProjectileTime;
    [SerializeField] private Transform _hand;
    [SerializeField] private Vector3 _handLocalPosition;
    [SerializeField] protected float _forwardForce;
    [SerializeField] protected float _verticalForce;

    private bool _projectileSpawned;
    private Projectile _currentProjectile;

    public override void Enter(BrainState overrideBrainState = null) {
        base.Enter(overrideBrainState);
        _animController.PlayAnimation(_attackName);
    }

    public override void Execute() {
        base.Execute();
        // wait for animation to start
        if (!_animController.IsStateByName(_attackName)) {
            return;
        }
        float currentTime = _animController.GetCurrentAnimationTime();
        if (currentTime >= 1f) {
            _npcBehaviour.ChangeBrainState(_targetOutOfRangeState);
        }
        if (currentTime > _fireProjectileTime) {
            // throw projectile
        } else if(currentTime > _spawnProjectileTime  && !_projectileSpawned) {
            // create projectile and place in hand
            PrepareProjectile();
        }
    }

    // prepare projectile
    private void PrepareProjectile() {
        // spawn projectile
        _currentProjectile = _projectilePool[Random.Range(0, _projectilePool.Length)];
        _currentProjectile.gameObject.SetActive(true);
        // place in appropriate location
        _currentProjectile.transform.parent = _hand;
        _currentProjectile.transform.localPosition = _handLocalPosition;

        _projectileSpawned = true;
    }

    // launches projectile at given vector
    private void FireProjectile() {
        Transform body = _npcBehaviour.BodyTransform;
        Vector3 velocity = body.forward * _forwardForce + body.up * _verticalForce;
        _currentProjectile.FireProjectile(velocity);
    }
}
