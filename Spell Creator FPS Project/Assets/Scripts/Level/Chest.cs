using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IRaycastInteractable {

    [SerializeField] private string _chestId;
    [SerializeField] private MeshRenderer[] meshRenderer; // allows chest visuals to be edited when tier is determined
    [SerializeField] private float _openAngle;
    [SerializeField] private Transform _hinge;
    [SerializeField] private Transform _rewardsSpawn;

    public event InteractEvent OnInteractAttempt;
    public event InteractEvent OnInteractSuccess;

    public bool Interactable { get; private set; }
    [SerializeField] private BoxCollider _collider;
    public Vector3 InteractableCenter => transform.position + _collider.center;

    [SerializeField] private string _overrideId;
    public string OverrideId => _overrideId;
    public string InteractableId => _chestId;

    public void Detect(CharacterBehaviour character) {
        
    }

    public void Undetect() {

    }

    public void Initialize(ChestType type, string chestId) {
        if (!_overrideId.Equals(string.Empty)) {
            _chestId = _overrideId;
        } else {
            _chestId = chestId;
        }
        // TODO: edit chest visuals when initialized
        Interactable = true;
        LevelManager.LevelManagerInstance.RegisterInteractable(this);
    }

    public void InteractPress(CharacterBehaviour character) {
        if(character == PlayerController.Instance) {
            Open();
        }
    }

    public void InteractHold(CharacterBehaviour character) {

    }

    private void Open() {
        OnInteractAttempt?.Invoke();
        if (!Interactable) {
            return;
        }
        Interactable = false;
        RewardsSet rewards = LootManager.Instance.GetRewards(_chestId);
        // play chest open animation
        // at appropriate time, spawn reward objects
        _hinge.eulerAngles = new Vector3(_openAngle, 0f, 0f);

        OnChestOpened(rewards); // temp until animations are in
        OnInteractSuccess?.Invoke();
    }

    private void OnChestOpened(RewardsSet rewards) {
        SpawnRewards(rewards);
    }

    private void SpawnRewards(RewardsSet rewards) {
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

    private void SpawnRecoveryOrb(RecoveryOrbType recoveryOrbType) {
        PooledObject obj;
        if(!PooledObjectManager.Instance.UsePooledObject(GameplayValues.ObjectPooling.RecoveryOrbPrefabId, out obj)) {
            return;
        }
        RecoveryOrb recoveryOrb = obj as RecoveryOrb;
        if (recoveryOrb != null) {
            recoveryOrb.ActivatePooledObject();
            recoveryOrb.Initialize(recoveryOrbType);
            recoveryOrb.transform.position = _rewardsSpawn.position;
        }
    }

    private void SpawnInventoryItem(string itemId) {
        PooledObject obj;
        if (!PooledObjectManager.Instance.UsePooledObject(GameplayValues.ObjectPooling.WorldRunePrefabId, out obj)) {
            return;
        }
        Rune rune = obj as Rune;
        if (rune != null) {
            rune.transform.position = _rewardsSpawn.position;
            rune.Initialize(itemId);
        }
    }
}
