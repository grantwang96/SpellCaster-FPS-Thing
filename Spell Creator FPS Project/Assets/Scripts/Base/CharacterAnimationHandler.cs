using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterAnimationHandler : MonoBehaviour { // base class that handles animations

    [SerializeField] protected Animator _anim;
    [SerializeField] protected Transform _bodyRoot;
    [SerializeField] protected CharacterBehaviour _characterBehaviour;

    [SerializeField] protected AnimationCurve _accelerationTiltCurve;
    [SerializeField] protected float _tiltLimit;
    [SerializeField] private bool _tiltEnabled;

    public event Action<AnimationState> OnAnimationStateUpdated;
    
    protected virtual void Start() {
        _characterBehaviour.Damageable.OnStun += OnStun;
        _characterBehaviour.Damageable.OnKnockback += OnKnockback;
    }
    
    public virtual void TiltCharacter(float x) {
        if (!_tiltEnabled) { return; }
        float val = Mathf.Clamp(x, -1, 1);
        // int mod = x > 0 ? 1 : -1;
        // float val = Mathf.Abs(accelerationTiltCurve.Evaluate(x));
        _bodyRoot.localEulerAngles = new Vector3(
            _bodyRoot.localEulerAngles.x, _bodyRoot.localEulerAngles.y, -val * _tiltLimit);
    }

    public virtual bool IsStateByName(string name) {
        return _anim.GetCurrentAnimatorStateInfo(0).IsName(name);
    }

    public virtual bool IsStateByTag(string tag) {
        return _anim.GetCurrentAnimatorStateInfo(0).IsTag(tag);
    }

    public virtual float GetCurrentAnimationTime() {
        return _anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public virtual float GetCurrentAnimationDuration() {
        return _anim.GetCurrentAnimatorStateInfo(0).length;
    }

    public virtual void UpdateAnimationData(AnimationData data) {
        if (!string.IsNullOrEmpty(data.AnimationName)) {
            _anim.Play(data.AnimationName);
        }
        for(int i = 0; i < data.Bools.Count; i++) {
            _anim.SetBool(data.Bools[i].Key, data.Bools[i].Value);
        }
        for(int i = 0; i < data.Ints.Count; i++) {
            _anim.SetInteger(data.Ints[i].Key, data.Ints[i].Value);
        }
        for(int i = 0; i < data.Floats.Count; i++) {
            _anim.SetFloat(data.Floats[i].Key, data.Floats[i].Value);
        }
        for (int i = 0; i < data.Triggers.Count; i++) {
            _anim.SetTrigger(data.Triggers[i]);
        }
        for (int i = 0; i < data.ResetTriggers.Count; i++) {
            _anim.ResetTrigger(data.ResetTriggers[i]);
        }
    }

    public virtual void SetAnimatorEnabled(bool enabled) {
        _anim.enabled = enabled;
    }

    protected void UpdateAnimationState(AnimationState state) {
        OnAnimationStateUpdated?.Invoke(state);
    }

    private void OnTakeDamage(DamageData data) {
        _anim.SetFloat("Horizontal", data.Direction.normalized.x);
        _anim.SetFloat("Vertical", data.Direction.normalized.z);
    }

    private void OnStun(Vector3 direction, float power) {
        Vector3 localDirection = _characterBehaviour.BodyTransform.InverseTransformDirection(direction);
        _anim.SetFloat("Horizontal", localDirection.x);
        _anim.SetFloat("Vertical", localDirection.z);
        _anim.SetTrigger("Stun");
    }

    private void OnKnockback(Vector3 direction, float power) {
        _anim.SetTrigger("Knockback");
    }

    private void OnDeath(bool isDead, Damageable damageable) {
        
    }
}

public enum AnimationState {
    Started,
    InProgress,
    CanTransition,
    Completed
}
