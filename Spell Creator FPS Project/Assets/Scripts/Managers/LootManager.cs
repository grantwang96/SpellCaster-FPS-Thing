﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LootTier {
    White = 0,
    Green = 1,
    Purple = 2,
    Gold = 3
}

public enum ChestType {
    Wood = 0,
    Metal = 1,
    Gold = 2,
    Jeweled = 3
}

public interface ILootManager {

    RewardsSet OpenChest(string chestId);
}

public class LootManager : MonoBehaviour, ILootManager {

    private const string ChestIdPrefix = "FunBoxProbablyFullOfCokeAndHookers_";
    private const string ResourceLocation = "Config/data.ChestConfig";
    private const int ChestIdLength = 20;

    private static LootManager _instance;
    public static ILootManager Instance => _instance;

    public ChestConfig ChestConfig { get; private set; }

    // list of all chests that appear in the level
    private Dictionary<string, ChestInfo> _chests = new Dictionary<string, ChestInfo>();

    [SerializeField] private List<string> _whiteTierLoot = new List<string>();
    [SerializeField] private List<string> _greenTierLoot = new List<string>();
    [SerializeField] private List<string> _purpleTierLoot = new List<string>();
    [SerializeField] private List<string> _goldTierLoot = new List<string>();

    [SerializeField] private string _runePrefabId;
    [SerializeField] private string _recoveryOrbPrefabId;

	// Use this for initialization
	private void Awake () {
        _instance = this;
        LoadConfig();
	}

    private void Start() {
        PopulateLevel();
    }

    private void LoadConfig() {
        Object obj = Resources.Load(ResourceLocation, typeof(ChestConfig));
        ChestConfig = obj as ChestConfig;
        if(ChestConfig != null) {
            Debug.Log("Successfully loaded chest config!");
        }
    }

    // TODO: Get list of all locations for chest in level data
    // called at start. Populates the level with chests based on location type and rarity
    private void PopulateLevel() {
        // for each possible location in the level
        for(int i = 0; i < LevelManager.Instance.ChestLocations.Count; i++) {
            ChestSpawn chestSpawn = LevelManager.Instance.ChestLocations[i];
            string id = RegisterChest(chestSpawn.ChestType);
            chestSpawn.SpawnPrefab(id);
        }
    }

    // called when player opens chest
    public RewardsSet OpenChest(string chestId) {
        if (!_chests.ContainsKey(chestId)) {
            Debug.LogError($"Could not retrieve chest info with given id {chestId}!");
            return RewardsSet.Default;
        }
        ChestInfo chestInfo = _chests[chestId];
        _chests.Remove(chestId);
        RewardsSet rewards = GenerateChestRewards(chestInfo.ChestType);
        return rewards;
    }

    private RewardsSet GenerateChestRewards(ChestType chestType) {
        List<string> inventoryRewards = new List<string>();
        // get each loot tier amount using config scriptable object
        ChestTypeData[] chestTypes = ChestConfig.ChestTypes;
        ChestTypeData data = null;
        for(int i = 0; i < chestTypes.Length; i++) {
            if(chestTypes[i].ChestType == chestType) {
                data = chestTypes[i];
                continue;
            }
        }
        if(data == null) {
            Debug.LogError($"Could not retrieve chest config data for chest type {chestType}!");
            return RewardsSet.Default;
        }
        // fill list with inventory storable ids
        int tierRewardsCount = Random.Range(data.WhiteTier.Min, data.WhiteTier.Max);
        for(int i = 0; i < tierRewardsCount; i++) {
            inventoryRewards.Add(_whiteTierLoot[Random.Range(0, _whiteTierLoot.Count)]);
        }
        tierRewardsCount = Random.Range(data.GreenTier.Min, data.GreenTier.Max);
        for (int i = 0; i < tierRewardsCount; i++) {
            inventoryRewards.Add(_greenTierLoot[Random.Range(0, _greenTierLoot.Count)]);
        }
        tierRewardsCount = Random.Range(data.PurpleTier.Min, data.PurpleTier.Max);
        for (int i = 0; i < tierRewardsCount; i++) {
            inventoryRewards.Add(_purpleTierLoot[Random.Range(0, _purpleTierLoot.Count)]);
        }
        tierRewardsCount = Random.Range(data.GoldTier.Min, data.GoldTier.Max);
        for (int i = 0; i < tierRewardsCount; i++) {
            inventoryRewards.Add(_goldTierLoot[Random.Range(0, _goldTierLoot.Count)]);
        }
        int healthOrbs = Random.Range(data.HealthOrbs.Min, data.HealthOrbs.Max);
        int manaOrbs = Random.Range(data.ManaOrbs.Min, data.ManaOrbs.Max);
        return new RewardsSet(healthOrbs, manaOrbs, inventoryRewards);
    }

    // creates a chest based on tier and returns the Id
    private string RegisterChest(ChestType chestType) {
        string chestId = GenerateChestId();
        ChestInfo chestInfo = new ChestInfo(chestId, chestType);
        _chests.Add(chestId, chestInfo);
        return chestId;
    }

    private string GenerateChestId() {
        string chestId = $"{ChestIdPrefix}{StringGenerator.RandomString(ChestIdLength)}";
        return chestId;
    }
}
