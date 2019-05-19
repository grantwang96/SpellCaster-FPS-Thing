using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable {

    [SerializeField] private string _chestId;
    [SerializeField] private MeshRenderer[] meshRenderer; // allows chest visuals to be edited when tier is determined
    [SerializeField] private float _openAngle;
    [SerializeField] private Transform _rewardsSpawn;

    public bool Interactable { get; private set; }

    public void Detect() {
        throw new System.NotImplementedException();
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

        OnChestOpened(rewards); // temp until animations are in
    }

    private void OnChestOpened(RewardsSet rewards) {
        SpawnRewards(rewards);
    }

    private void SpawnRewards(RewardsSet rewards) {
        for(int i = 0; i < rewards.HealthOrbs; i++) {
            PooledObject obj = ObjectPool.Instance.UsePooledObject(GameplayValues.ObjectPooling.RecoveryOrbPrefabId);
            // TODO: CREATE RECOVERY ORB COMPONENT
        }
        for(int i = 0; i < rewards.ManaOrbs; i++) {

        }
        for(int i = 0; i < rewards.InventoryItems.Count; i++) {

        }
    }

    // Use this for initialization
    void Start () {
		
	}
}
