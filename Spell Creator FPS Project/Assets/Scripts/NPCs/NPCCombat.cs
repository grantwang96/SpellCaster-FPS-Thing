using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPCCombat : MonoBehaviour {

    [SerializeField] protected NPCDamageable _damageable;
    [SerializeField] protected NPCAnimController _animController;
    [SerializeField] protected NPCBehaviour _behaviour;

    protected AttackPhase[] AttackPhases => _behaviour.Blueprint.AttackPhases;
    public float CurrentAnimationTime {
        get {
            return _animController.GetCurrentAnimationTime();
        }
    }
    public abstract string CurrentAttackName { get; }

    protected bool _attackTriggered;

    public delegate void AttackStartedDelegate();
    public event AttackStartedDelegate OnAttackStarted;
    public delegate void AttackFinishedDelegate();
    public event AttackFinishedDelegate OnAttackFinished;

    /// <summary>
    /// Initiates attack sequence with specified name
    /// </summary>
    /// <param name="attackName"></param>
    public abstract bool Attack(string attackName);

    /// <summary>
    /// Function that decides next attack
    /// </summary>
    /// <param name="previousAttack"></param>
    /// <returns></returns>
    public abstract string NextAttack();

    protected virtual void StartAttack(string attackName = null) {
        // if this is a combo attack, set a trigger. Otherwise, it's a "special" attack
        if (string.IsNullOrEmpty(attackName)) {
            _animController.SetTrigger("Attack");
        } else {
            _animController.PlayAnimation(attackName);
        }
        OnAttackStarted?.Invoke();
    }

    protected virtual void EndAttack() {
        OnAttackFinished?.Invoke();
    }

    protected abstract IEnumerator ProcessAttack();
}

public interface IAttackData {
    string AttackName { get; }
}