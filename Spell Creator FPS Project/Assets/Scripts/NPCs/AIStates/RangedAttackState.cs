using System.Collections;
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
    
    [SerializeField] private Transform _hand;
    [SerializeField] private Vector3 _handLocalPosition;
    [SerializeField] protected float _forwardForce;
    [SerializeField] protected float _verticalForce;
    [SerializeField] protected float _lifeTime;
    
    private Projectile _currentProjectile;

    protected override void Awake() {
        base.Awake();
        PooledObjectManager.Instance.RegisterPooledObject(_projectilePrefabId, 10);
    }

    public override void Enter(BrainState overrideBrainState = null, float duration = 0f) {
        base.Enter(overrideBrainState, duration);
        _moveController.SetRotation(_npcVision.CurrentTarget.transform.position, .9f);
    }

    public override void Exit() {
        base.Exit();
        if(_currentProjectile != null) {
            _currentProjectile.DeactivatePooledObject();
        }
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

    public override bool CanTransition() {
        return !TargetTooClose() && TargetWithinRange(_npcVision.CurrentTarget.GetBodyPosition());
    }

    private bool TargetTooClose() {
        return Vector3.Distance(_npcBehaviour.BodyTransform.position, _npcVision.CurrentTarget.BodyTransform.position) < _meleeRange;
    }

    private void OnAttackFinish() {
        Vector3 targetPosition = _npcVision.CurrentTarget.GetBodyPosition();
        if (_npcVision.CanSeeTarget(targetPosition) && TargetWithinRange(targetPosition)) {
            _npcBehaviour.ChangeBrainState(_targetInRangeState);
        } else if (TargetTooClose()) {
            _npcBehaviour.ChangeBrainState(_targetMeleeRangeState);
        } else {
            _npcBehaviour.ChangeBrainState(_targetOutOfRangeState);
        }
    }

    protected override void OnAnimationStateUpdated(AnimationState state) {
        base.OnAnimationStateUpdated(state);
        if (state == AnimationState.Started) {
            PrepareProjectile();
        } else if(state == AnimationState.InProgress){
            FireProjectile();
        } else if(state == AnimationState.CanTransition) {

        } else if(state == AnimationState.Completed) {
            OnAttackFinish();
        }
    }
}
