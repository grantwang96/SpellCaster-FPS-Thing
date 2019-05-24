using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable {

    [SerializeField] private string _chestId;
    [SerializeField] private MeshRenderer[] meshRenderer; // allows chest visuals to be edited when tier is determined
    [SerializeField] private float _openAngle;
    [SerializeField] private Transform _hinge;
    [SerializeField] private Transform _rewardsSpawn;

    public bool Interactable { get; private set; }

    public void Detect() {
        
    }

    public void Initialize(ChestType type, string chestId) {
        _chestId = chestId;
        // TODO: edit chest visuals when initialized
        Interactable = true;
    }

    public void Interact(CharacterBehaviour character) {
        if(character == GameplayController.Instance) {
            Open();
        }
    }

    private void Open() {
        if (!Interactable) {
            return;
        }
        Interactable = false;
        RewardsSet rewards = LootManager.Instance.OpenChest(_chestId);
        // play chest open animation
        // at appropriate time, spawn reward objects
        _hinge.eulerAngles = new Vector3(_openAngle, 0f, 0f);

        OnChestOpened(rewards); // temp until animations are in
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

    // Use this for initialization
    void Start () {
		
	}
}
