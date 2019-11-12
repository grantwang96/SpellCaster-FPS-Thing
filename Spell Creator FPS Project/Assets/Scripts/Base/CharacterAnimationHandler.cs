using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationHandler : MonoBehaviour { // base class that handles animations

    [SerializeField] protected Animator _anim;
    [SerializeField] protected Transform _bodyRoot;
    [SerializeField] protected CharacterBehaviour _characterBehaviour;
    [SerializeField] protected CharacterMoveController _moveController;
    [SerializeField] protected Damageable _damageable;

    [SerializeField] protected AnimationCurve _accelerationTiltCurve;
    [SerializeField] protected float _tiltLimit;
    [SerializeField] private bool _tiltEnabled;
    
    protected virtual void Start() {
        _damageable.OnHealthChanged += OnHealthChanged;
        _damageable.OnDeath += OnDeath;
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

    public virtual void SetIntParameter(string parameter, int num) {
        if (_anim.GetInteger(parameter) != -1) {
            _anim.SetInteger(parameter, num);
        }
    }

    public virtual void SetTrigger(string triggerName) {
        _anim.SetTrigger(triggerName);
    }

    public virtual void ResetTrigger(string triggerName) {
        _anim.ResetTrigger(triggerName);
    }

    public virtual void PlayAnimation(string animationName) {
        _anim.Play(animationName);
    }

    public virtual void StopAnimation() {
        _anim.StopPlayback();
    }

    protected virtual void OnHealthChanged(int health) {
        // determine if damage was taken
        // immediately enter damaged animation state
    }

    protected virtual void OnDeath(bool isDead, Damageable damageable) {
        // immediately enter death state
    }
}
