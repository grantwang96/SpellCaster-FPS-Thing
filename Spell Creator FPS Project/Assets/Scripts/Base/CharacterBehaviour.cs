using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// The brain of a given character
/// This object should have access to all relevant components of the character in order to process and execute various actions
/// </summary>
public abstract class CharacterBehaviour : MonoBehaviour {

    /// <summary>
    /// The "intended" vector that the character wants to move
    /// </summary>
    [SerializeField] protected Vector3 _moveVector;
    public Vector3 MoveVector { get { return _moveVector; } }
    [SerializeField] public float BaseSpeed { get; protected set; }
    [SerializeField] public float MaxSpeed { get; protected set; }
    public abstract float GetMoveMagnitude();

    [SerializeField] protected Transform _bodyTransform;
    public Transform BodyTransform { get { return _bodyTransform; } }
    public virtual Vector3 GetBodyPosition() {
        return BodyTransform.position;
    }

    // where vision is calculated
    [SerializeField] protected Transform _headTransform;
    public Transform Head { get { return _headTransform; } }

    [SerializeField] protected CharacterAnimationHandler _animHandler;
    public CharacterAnimationHandler CharacterAnimationHandler {
        get {
            return _animHandler;
        }
    }
    public delegate void ChangeAnimationStateCallback(string stateName, params int[] args);
    public event ChangeAnimationStateCallback ChangeAnimationState;

    [SerializeField] protected List<string> _unitTags = new List<string>(); // can dynamically change what type of tags exist on this character
    public List<string> UnitTags => _unitTags;
    
    protected virtual void Awake() {
        _animHandler = GetComponent<CharacterAnimationHandler>();
    }

    protected void InvokeChangeAnimationState(string stateName) {
        ChangeAnimationState?.Invoke(stateName);
    }
}
