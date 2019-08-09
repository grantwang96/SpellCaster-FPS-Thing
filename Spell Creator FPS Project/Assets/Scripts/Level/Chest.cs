using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable {

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

    public void Detect() {
        
    }

    public void Initialize(ChestType type, string chestId) {
        if (!_overrideId.Equals(string.Empty)) {
            _chestId = _overrideId;
        } else {
            _chestId = chestId;
        }
        // TODO: edit chest visuals when initialized
        Interactable = true;
        LevelManager.Instance.RegisterInteractable(this);
    }

    public void InteractPress(CharacterBehaviour character) {
        if(character == GameplayController.Instance) {
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
        RewardsSet rewards = LootManager.Instance.OpenChest(_chestId);
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

        }
    }

    private void SpawnRecoveryOrb(RecoveryOrbType recoveryOrbType) {
        PooledObject obj = ObjectPool.Instance.UsePooledObject(GameplayValues.ObjectPooling.RecoveryOrbPrefabId);
        RecoveryOrb recoveryOrb = obj as RecoveryOrb;
        Debug.Log("Spawning recovery orb");
        if (recoveryOrb != null) {
            recoveryOrb.ActivatePooledObject();
            recoveryOrb.Initialize(recoveryOrbType);
            recoveryOrb.transform.position = _rewardsSpawn.position;
        }
    }
}
