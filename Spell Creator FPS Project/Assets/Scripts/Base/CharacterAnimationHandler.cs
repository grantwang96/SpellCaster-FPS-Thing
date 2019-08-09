using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationHandler : MonoBehaviour { // base class that handles animations

    public Animator anim;
    public Transform bodyRoot;
    protected CharacterBehaviour characterBehaviour;

    public AnimationCurve accelerationTiltCurve;
    public float tiltLimit;
    [SerializeField] private bool tiltEnabled;

    protected virtual void Awake() {
        characterBehaviour = GetComponent<CharacterBehaviour>();
        characterBehaviour.ChangeAnimationState += OnStateChange;
    }

    protected virtual void Start() {
        bodyRoot = characterBehaviour.BodyTransform;
        anim = bodyRoot.GetComponent<Animator>();
    }

    protected virtual void OnDestroy() {
        characterBehaviour.ChangeAnimationState -= OnStateChange;
    }

    public virtual void TiltCharacter(float x) {
        if (!tiltEnabled) { return; }
        float val = Mathf.Clamp(x, -1, 1);
        // int mod = x > 0 ? 1 : -1;
        // float val = Mathf.Abs(accelerationTiltCurve.Evaluate(x));
        bodyRoot.localEulerAngles = new Vector3(
            bodyRoot.localEulerAngles.x, bodyRoot.localEulerAngles.y, -val * tiltLimit);
    }

    public virtual bool IsStateByName(string name) {
        return anim.GetCurrentAnimatorStateInfo(0).IsName(name);
    }

    public virtual bool IsStateByTag(string tag) {
        return anim.GetCurrentAnimatorStateInfo(0).IsTag(tag);
    }

    public virtual float GetCurrentAnimationTime() {
        return anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public virtual float GetCurrentAnimationDuration() {
        return anim.GetCurrentAnimatorStateInfo(0).length;
    }

    public virtual void SetIntParameter(string parameter, int num) {
        if (anim.GetInteger(parameter) != -1) {
            anim.SetInteger(parameter, num);
        }
    }

    public virtual void SetTrigger(string triggerName) {
        anim.SetTrigger(triggerName);
    }

    public virtual void ResetTrigger(string triggerName) {
        anim.ResetTrigger(triggerName);
    }

    public virtual void PlayAnimation(string animationName) {
        anim.Play(animationName);
    }

    public virtual void StopAnimation() {
        anim.StopPlayback();
    }

    protected virtual void OnStateChange(string stateName, params int[] args) {
        /*
        if(stateName == string.Empty) {
            return;
        }
        anim.SetTrigger(stateName);
        */
    }
}
