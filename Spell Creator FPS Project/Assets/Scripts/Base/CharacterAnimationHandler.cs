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

    protected virtual void Start() {
        characterBehaviour = GetComponent<CharacterBehaviour>();
        characterBehaviour.ChangeAnimationState += OnStateChange;
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

    public virtual void PlayAnimation(string animName) {
        anim.Play(animName);
    }

    public virtual void StopCurrentAnimation() {
        anim.StopPlayback();
    }

    public virtual void SetTrigger(string triggerName) {
        anim.SetTrigger(triggerName);
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

    public virtual string GetCurrentAnimationStateName() {
        return anim.GetCurrentAnimatorStateInfo(0).ToString();
    }

    public virtual void SetIntParameter(string parameter, int num) {
        if (anim.GetInteger(parameter) != -1) {
            anim.SetInteger(parameter, num);
        }
    }

    protected virtual void OnStateChange(string stateName, params int[] args) {
        if(stateName == string.Empty) {
            return;
        }
        anim.SetTrigger(stateName);
    }
}
