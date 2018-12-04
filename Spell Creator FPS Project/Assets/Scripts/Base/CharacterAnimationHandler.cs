using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationHandler : MonoBehaviour { // base class that handles animations

    public Animator anim;
    public Transform bodyCore;

    public AnimationCurve accelerationTiltCurve;
    public float tiltLimit;
    [SerializeField] private bool tiltEnabled;

    private void Start() {

    }

    private void Update() {
        
    }

    public virtual void TiltCharacter(float x) {
        if (!tiltEnabled) { return; }
        float val = Mathf.Clamp(x, -1, 1);
        // int mod = x > 0 ? 1 : -1;
        // float val = Mathf.Abs(accelerationTiltCurve.Evaluate(x));
        bodyCore.localEulerAngles = new Vector3(
            bodyCore.localEulerAngles.x, bodyCore.localEulerAngles.y, -val * tiltLimit);
    }

    public virtual void InitializeCharacterAnimations() {

    }

    /// <summary>
    /// Method that will override and save animation controller
    /// </summary>
    /// <param name="overrideClips"></param> // a dictionary that has contains old clips and the new clips that will replace them
    public virtual void OverrideCharacterAnimations(Dictionary<AnimationClip, AnimationClip> overrideClips) {
        AnimatorOverrideController aoc = new AnimatorOverrideController(anim.runtimeAnimatorController);
        var animClips = new List<KeyValuePair<AnimationClip, AnimationClip>>();

        foreach (AnimationClip a in aoc.animationClips) {
            AnimationClip clip = a;
            if (overrideClips.ContainsKey(clip)) {
                clip = overrideClips[clip];
            }
            animClips.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, clip));
        }
        aoc.ApplyOverrides(animClips);
        anim.runtimeAnimatorController = aoc;
    }

    public virtual void RetrieveAnimations() {
        
    }

    public virtual void PlayAnimationByTrigger(string triggerName, params int[] args) {

    }

    public virtual void PlayAnimationByName(string animationName, params int[] args) {

    }
    
    protected virtual void OnCharacterIdle() {

    }

    public virtual bool IsStateByName(string name) {
        return anim.GetCurrentAnimatorStateInfo(0).IsName(name);
    }

    public virtual bool IsStateByTag(string tag) {
        return anim.GetCurrentAnimatorStateInfo(0).IsTag(tag);
    }
}
