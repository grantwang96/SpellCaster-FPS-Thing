using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackState : AttackState {

    [SerializeField] private BrainState _targetInRangeState;
    [SerializeField] private BrainState _targetOutOfRangeState;
    [SerializeField] private BrainState _targetMeleeRangeState;

    [SerializeField] private GameObject[] _projectilePool;
    [SerializeField] private float _spawnProjectileTime;
    [SerializeField] private float _fireProjectileTime;

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
            // generate visual of projectile
        } else if(currentTime > _spawnProjectileTime) {
            // create projectile
        }
    }

    // prepare projectile
    private void PrepareProjectile() {
        // spawn projectile
        // place in appropriate location
    }

    // launches projectile at given vector
    private void FireProjectile() {

    }
}
