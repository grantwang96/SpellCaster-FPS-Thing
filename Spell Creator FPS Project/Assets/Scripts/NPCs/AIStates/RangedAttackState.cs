﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this will likely become a parent class for more ranged attacks
public class RangedAttackState : AttackState {

    private const string CombatPooledObjectsResourcesPath = "Combat";

    [SerializeField] private BrainState _targetInRangeState;
    [SerializeField] private BrainState _targetOutOfRangeState;
    [SerializeField] private BrainState _targetMeleeRangeState;

    [SerializeField] private float _meleeRange;

    // configurable data for projectiles
    [SerializeField] protected string _projectilePrefabId;
    [SerializeField] protected float _power = 1f;
    [SerializeField] protected bool _useGravity;
    [SerializeField] protected Effect[] _effects;

    [SerializeField] private float _spawnProjectileTime;
    [SerializeField] private float _fireProjectileTime;
    [SerializeField] private Transform _hand;
    [SerializeField] private Vector3 _handLocalPosition;
    [SerializeField] protected float _forwardForce;
    [SerializeField] protected float _verticalForce;
    [SerializeField] protected float _lifeTime;

    private bool _projectileSpawned;
    private Projectile _currentProjectile;

    protected override void Awake() {
        base.Awake();
        PooledObjectManager.Instance.RegisterPooledObject(_projectilePrefabId, 10);
    }

    protected override void SetTriggerName() {
        _triggerName = GameplayValues.BrainStates.RangeAttackStateId;
    }


    public override void Enter(BrainState overrideBrainState = null, float duration = 0f) {
        base.Enter(overrideBrainState, duration);
        _animController.PlayAnimation(_attackName);
        _moveController.SetRotation(_npcVision.CurrentTarget.transform.position, .9f);
    }

    public override void Execute() {
        base.Execute();
        // wait for animation to start
        if (!_animController.IsStateByName(_attackName)) {
            return;
        }
        float currentTime = _animController.GetCurrentAnimationTime();
        if(currentTime >= 1f) {
            OnAttackFinish();
            return;
        }
        if (currentTime > _fireProjectileTime && _currentProjectile != null) {
            // throw projectile
            FireProjectile();
        } else if(currentTime > _spawnProjectileTime  && !_projectileSpawned) {
            // create projectile and place in hand
            PrepareProjectile();
        }
    }

    public override void Exit() {
        base.Exit();
        if(_currentProjectile != null) {
            _currentProjectile.DeactivatePooledObject();
        }
        _projectileSpawned = false;
    }

    // prepare projectile
    protected virtual void PrepareProjectile() {
        // spawn projectile
        PooledObject pooledObject;
        if(!PooledObjectManager.Instance.UsePooledObject(_projectilePrefabId, out pooledObject)) {
            return;
        }
        _currentProjectile = pooledObject as Projectile;
        if(_currentProjectile == null) {
            Debug.LogError($"Object {pooledObject} retrieved with ID \"{_projectilePrefabId}\" is not of type Projectile");
            return;
        }
        // place in appropriate location
        _currentProjectile.InitializeOwner(_npcBehaviour.Damageable);
        _currentProjectile.transform.parent = _hand;
        _currentProjectile.transform.localPosition = _handLocalPosition;
        _currentProjectile.ActivatePooledObject();
        _currentProjectile?.InitializeProjectile(_power, _lifeTime, _effects);

        _projectileSpawned = true;
    }

    // launches projectile at given vector
    protected virtual void FireProjectile() {
        if (_currentProjectile == null || _currentProjectile.IsLive) { return; }
        Vector3 forward = _npcVision.CurrentTarget.GetBodyPosition() - _hand.position;
        Vector3 velocity = forward.normalized * _forwardForce + _npcBehaviour.transform.up * _verticalForce;
        _currentProjectile.FireProjectile(_useGravity, velocity);
        // TODO: Calculate the velocity required to send the object directly at the target
        _currentProjectile = null;
    }

    private bool TargetTooClose() {
        return Vector3.Distance(_npcBehaviour.BodyTransform.position, _npcVision.CurrentTarget.BodyTransform.position) < _meleeRange;
    }

    private void OnAttackFinish() {
        if (_npcVision.CanSeeTarget(_npcVision.CurrentTarget.GetBodyPosition())) {
            _npcBehaviour.ChangeBrainState(_targetInRangeState);
        } else if (TargetTooClose()) {
            _npcBehaviour.ChangeBrainState(_targetMeleeRangeState);
        } else {
            _npcBehaviour.ChangeBrainState(_targetOutOfRangeState);
        }
    }
}
