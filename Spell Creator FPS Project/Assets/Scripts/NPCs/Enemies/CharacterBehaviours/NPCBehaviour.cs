using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NPCMoveController))]
public class NPCBehaviour : CharacterBehaviour {

    [SerializeField] protected string _id;
    public string Id => _id;
    
    public NPCMoveController CharMove { get; private set; }
    [SerializeField] protected Damageable _damageable;
    public override Damageable Damageable => _damageable;
    [SerializeField] protected NPCBlueprint _blueprint; // blueprint to derive data from
    public NPCBlueprint Blueprint => _blueprint;

    [SerializeField] protected BrainState _currentBrainState; // current state of AI State Machine
    [SerializeField] protected BrainState _startingState; // how the NPC should behave at start (usually idle)

    [SerializeField] private List<BrainStateTransition> _allBrainStateTransitions = new List<BrainStateTransition>();
    private Dictionary<BrainStateTransitionId, BrainState> _brainStateTransitions = new Dictionary<BrainStateTransitionId, BrainState>();
    
    public event Action<string> OnBrainStateChanged;

    public event Action OnCharacterSpawned;

    protected bool _prepped = false;

    protected override void Awake() {
        base.Awake();
        InitializeBrainStateTransitions();
        CharMove = _moveController as NPCMoveController;
        _prepped = true;
    }

    protected virtual void InitializeBrainStateTransitions() {
        for(int i = 0; i < _allBrainStateTransitions.Count; i++) {
            _brainStateTransitions.Add(_allBrainStateTransitions[i].TransitionId, _allBrainStateTransitions[i].BrainState);
        }
    }

    protected void GenerateUniqueId(string overrideUniqueId) {
        if (!string.IsNullOrEmpty(overrideUniqueId)) {
            _id = overrideUniqueId;
            return;
        }
        _id = $"{_blueprint.NpcIdPrefix}_{StringGenerator.RandomString(GameplayValues.Combat.NPCIdSize)}";
    }

    protected virtual void Start() {
        if(_blueprint == null) {
            Debug.LogError($"[{nameof(NPCBehaviour)}] NPC {name} doesn't have a blueprint!");
            return;
        }
        if(_damageable == null) {
            Debug.LogError($"[{nameof(NPCBehaviour)}] NPC {name} does not contain damageable!");
            OnDeath(true, null);
            return;
        }
        _damageable.OnDeath += OnDeath;
        if(_currentBrainState == null) {
            ChangeBrainState(_startingState);
        }
    }

    protected virtual void Update() {
        if(_currentBrainState != null) { _currentBrainState.Execute(); }
    }

    protected virtual void FireCharacterSpawnedEvent() {
        OnCharacterSpawned?.Invoke();
    }

    /// <summary>
    /// Changes the current state in the AI State Machine
    /// </summary>
    /// <param name="brainState"></param>
    public virtual void ChangeBrainState(BrainState brainState, BrainState overrideBrainState = null, float duration = 0f) {
        // perform any exit operations from the previous state
        _currentBrainState?.Exit();

        // save the new brain state and enter
        _currentBrainState = brainState;
        _currentBrainState?.Enter(overrideBrainState, duration);

        // OnBrainStateChanged?.Invoke(_currentBrainState?.TriggerName);
    }

    public virtual bool ChangeBrainState(BrainStateTransitionId transitionId, float duration = 0f) {
        if (!_brainStateTransitions.ContainsKey(transitionId)) {
            return false;
        }
        ChangeBrainState(_brainStateTransitions[transitionId], null, duration);
        return true;
    }

    protected virtual void OnDeath(bool isDead, Damageable damageable) {
        ChangeBrainState(null, null);
        DropLoot();
    }

    protected virtual void DropLoot() {
        RewardsSet rewards = LootManager.Instance.GetRewards(Id);
        for(int i = 0; i < rewards.HealthOrbs; i++) {
            SpawnRecoveryOrb(RecoveryOrbType.Health);
        }
        for(int i = 0; i < rewards.ManaOrbs; i++) {
            SpawnRecoveryOrb(RecoveryOrbType.Mana);
        }
        for(int i = 0; i < rewards.InventoryItems.Count; i++) {
            SpawnInventoryItem(rewards.InventoryItems[i]);
        }
    }

    protected virtual void SpawnRecoveryOrb(RecoveryOrbType recoveryOrbType) {
        PooledObject obj;
        if (!PooledObjectManager.Instance.UsePooledObject(GameplayValues.ObjectPooling.RecoveryOrbPrefabId, out obj)) {
            CustomLogger.Error(this.name, $"Could not retrieve pooled object with id {GameplayValues.ObjectPooling.RecoveryOrbPrefabId}");
            return;
        }
        RecoveryOrb recoveryOrb = obj as RecoveryOrb;
        if (recoveryOrb != null) {
            recoveryOrb.ActivatePooledObject();
            recoveryOrb.Initialize(recoveryOrbType);
            recoveryOrb.transform.position = GetBodyPosition();
        }
    }

    protected virtual void SpawnInventoryItem(string itemId) {
        PooledObject obj;
        if (!PooledObjectManager.Instance.UsePooledObject(GameplayValues.ObjectPooling.WorldRunePrefabId, out obj)) {
            return;
        }
        Rune rune = obj as Rune;
        if(rune != null) {
            rune.transform.position = GetBodyPosition();
            rune.Initialize(itemId);
        }
    }

    public void OverrideUnitTags(List<string> overrideTags) {
        _unitTags.Clear();
        _unitTags.AddRange(overrideTags);
    }

    public bool IsAnEnemy(CharacterBehaviour behaviour) {
        for(int i = 0; i < behaviour.UnitTags.Count; i++) {
            if (!_unitTags.Contains(behaviour.UnitTags[i])) {
                return true;
            }
        }
        return false;
    }
}

public interface INPCInitializable {
    void Initialize(NPCBlueprint blueprint);
}