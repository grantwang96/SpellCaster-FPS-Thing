﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NPCMoveController))]
public class NPCBehaviour : CharacterBehaviour {

    [SerializeField] protected string _id;
    public string Id => _id;
    
    [SerializeField] protected NPCMoveController _charMove;
    public NPCMoveController CharMove => _charMove;
    [SerializeField] protected Damageable _damageable;
    public override Damageable Damageable => _damageable;
    [SerializeField] protected NPCBlueprint _blueprint; // blueprint to derive data from
    public NPCBlueprint Blueprint => _blueprint;

    [SerializeField] protected BrainState _currentBrainState; // current state of AI State Machine
    [SerializeField] protected BrainState _startingState; // how the NPC should behave at start (usually idle)
    public delegate void BrainStateChangeDelegate(string newStateName);
    public event BrainStateChangeDelegate OnBrainStateChanged;

    public event Action OnCharacterSpawned;

    protected bool _prepped = false;

    protected override void Awake() {
        base.Awake();
        _charMove = GetComponent<NPCMoveController>();
        _damageable = GetComponent<Damageable>();
        _prepped = true;
    }

    protected virtual void OnEnable() {
        GenerateUniqueId();
        LootManager.Instance.OnEnemySpawn(_id, this);
    }

    private void GenerateUniqueId() {
        _id = $"{_blueprint.NpcIdPrefix}_{StringGenerator.RandomString(0)}";
    }

    protected virtual void Start() {
        if(_blueprint == null) {
            Debug.LogError(name + " doesn't have a blueprint!");
            return;
        }
        BaseSpeed = _blueprint.WalkSpeed;
        MaxSpeed = _blueprint.RunSpeed;

        if(_damageable == null) {
            Debug.LogError($"[{nameof(NPCBehaviour)}] Does not contain damageable!");
            OnDeath(true, null);
            return;
        }
        _damageable.OnDeath += OnDeath;
        ChangeBrainState(_startingState);
    }

    protected virtual void Update() {
        if(_currentBrainState != null) { _currentBrainState.Execute(); }
    }

    protected virtual void FireCharacterSpawnedEvent() {
        OnCharacterSpawned?.Invoke();
    }

    public override float GetMoveMagnitude() {
        Vector3 vel = _charMove.CharacterController.velocity;
        vel.y = 0f;
        return vel.magnitude;
    }

    /// <summary>
    /// Changes the current state in the AI State Machine
    /// </summary>
    /// <param name="brainState"></param>
    public virtual void ChangeBrainState(BrainState brainState, BrainState overrideBrainState = null) {
        // perform any exit operations from the previous state
        _currentBrainState?.Exit();

        // save the new brain state and enter
        _currentBrainState = brainState;
        _currentBrainState?.Enter(overrideBrainState);

        string stateName = _currentBrainState?.TriggerName ?? string.Empty;
        InvokeChangeAnimationState(stateName);
    }

    protected virtual void OnDeath(bool isDead, Damageable damageable) {
        ChangeBrainState(null);
    }
}

public interface INPCInitializable {
    void Initialize(NPCBlueprint blueprint);
}