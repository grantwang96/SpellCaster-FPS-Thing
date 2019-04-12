using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BrainState : MonoBehaviour{

    [SerializeField] protected string _triggerName;
    public string TriggerName { get { return _triggerName; } }
    [SerializeField] protected BrainState[] _childrenStates; // sub actions that should also be processing along with this parent state
    [SerializeField] protected NPCBehaviour _npcBehaviour;

    public virtual void Enter(BrainState overrideBrainState = null) {
        Debug.Log(_npcBehaviour.name + " has entered " + ToString());
        foreach(BrainState brainState in _childrenStates) {
            brainState.Enter(overrideBrainState);
        }
    }
    public virtual void Execute() {
        foreach (BrainState brainState in _childrenStates) {
            brainState.Execute();
        }

    }
    public virtual void Exit() {
        // Apply any final changes/calculations before switching to new state
        foreach (BrainState brainState in _childrenStates) {
            brainState.Exit();
        }
    }
}

public class TransitionState : BrainState {

    public TransitionState(float time) : base() {

    }
}
