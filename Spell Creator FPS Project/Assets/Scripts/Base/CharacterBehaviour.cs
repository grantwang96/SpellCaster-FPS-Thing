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

    [SerializeField] protected Transform _bodyTransform;
    public Transform BodyTransform { get { return _bodyTransform; } }
    public virtual Vector3 GetBodyPosition() {
        return BodyTransform.position;
    }

    // where vision is calculated
    [SerializeField] protected Transform _headTransform;
    public Transform Head { get { return _headTransform; } }

    public abstract Damageable Damageable { get; }
    [SerializeField] protected CharacterAnimationHandler _animHandler;
    public CharacterAnimationHandler AnimHandler => _animHandler;
    [SerializeField] protected CharacterMoveController _moveController;
    public CharacterMoveController MoveController => _moveController;

    [SerializeField] protected List<string> _unitTags = new List<string>(); // can dynamically change what type of tags exist on this character
    public List<string> UnitTags => _unitTags;
    
    protected virtual void Awake() {
        // _animHandler = GetComponent<CharacterAnimationHandler>();
    }
}
