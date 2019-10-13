using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LootTier {
    White = 0,
    Green = 1,
    Purple = 2,
    Gold = 3,
    Recovery = 99,
}

public enum ChestType {
    Wood = 0,
    Metal = 1,
    Gold = 2,
    Jeweled = 3
}

public interface ILootManager {
    
    RewardsSet GetRewards(string id);
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
    private Dictionary<string, RewardsSet> _enemies = new Dictionary<string, RewardsSet>();

    private Dictionary<LootTier, List<string>> _lootTable = new Dictionary<LootTier, List<string>>();
    [SerializeField] private List<string> _allLoot = new List<string>(); // temp

    [SerializeField] private string _runePrefabId;
    [SerializeField] private string _recoveryOrbPrefabId;

	// Use this for initialization
	private void Awake () {
        _instance = this;
        LoadConfig();
	}

    private void Start() {
        BuildLootTable();
        PopulateLevel();
        SubscribeToEvents();
    }

    private void OnDestroy() {
        UnsubscribeToEvents();
    }

    private void BuildLootTable() {
        _lootTable.Add(LootTier.White, new List<string>());
        _lootTable.Add(LootTier.Green, new List<string>());
        _lootTable.Add(LootTier.Purple, new List<string>());
        _lootTable.Add(LootTier.Gold, new List<string>());
        for (int i = 0; i < _allLoot.Count; i++) {
            IInventoryStorable storable = InventoryRegistry.Instance.GetItemById(_allLoot[i]);
            ILootableItem lootable = storable as ILootableItem;
            if(lootable == null) {
                Debug.LogError($"[{nameof(LootManager)}] ID {_allLoot[i]} was not a {nameof(ILootableItem)} object");
                continue;
            }
            _lootTable[lootable.LootTier].Add(_allLoot[i]);
        }
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
        for(int i = 0; i < LevelManager.LevelManagerInstance.ChestLocations.Count; i++) {
            ChestSpawn chestSpawn = LevelManager.LevelManagerInstance.ChestLocations[i];
            string id = RegisterChest(chestSpawn.ChestType, chestSpawn.OverrideId);
            chestSpawn.SpawnPrefab(id);
        }
    }
    
    private void SubscribeToEvents() {
        NPCManager.Instance.OnNPCSpawned += OnNPCSpawned;
    }

    private void UnsubscribeToEvents() {
        NPCManager.Instance.OnNPCSpawned -= OnNPCSpawned;
    }

    // called when player opens chest
    public RewardsSet GetRewards(string id) {
        RewardsSet rewards;
        if (_chests.ContainsKey(id)) {
            ChestInfo chestInfo = _chests[id];
            _chests.Remove(id);
            rewards = GenerateChestRewards(chestInfo.ChestType);
            return rewards;
        } else if (_enemies.ContainsKey(id)) {
            rewards = _enemies[id];
            return rewards;
        }
        Debug.LogError($"Could not retrieve chest info with given id {id}!");
        return RewardsSet.Default;
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
            inventoryRewards.Add(_lootTable[LootTier.White][Random.Range(0, _lootTable[LootTier.White].Count)]);
        }
        tierRewardsCount = Random.Range(data.GreenTier.Min, data.GreenTier.Max);
        for (int i = 0; i < tierRewardsCount; i++) {
            inventoryRewards.Add(_lootTable[LootTier.Green][Random.Range(0, _lootTable[LootTier.Green].Count)]);
        }
        tierRewardsCount = Random.Range(data.PurpleTier.Min, data.PurpleTier.Max);
        for (int i = 0; i < tierRewardsCount; i++) {
            inventoryRewards.Add(_lootTable[LootTier.Purple][Random.Range(0, _lootTable[LootTier.Purple].Count)]);
        }
        tierRewardsCount = Random.Range(data.GoldTier.Min, data.GoldTier.Max);
        for (int i = 0; i < tierRewardsCount; i++) {
            inventoryRewards.Add(_lootTable[LootTier.Gold][Random.Range(0, _lootTable[LootTier.Gold].Count)]);
        }
        int healthOrbs = Random.Range(data.HealthOrbs.Min, data.HealthOrbs.Max);
        int manaOrbs = Random.Range(data.ManaOrbs.Min, data.ManaOrbs.Max);
        return new RewardsSet(healthOrbs, manaOrbs, inventoryRewards);
    }

    // creates a chest based on tier and returns the Id
    private string RegisterChest(ChestType chestType, string overrideId) {
        string chestId = overrideId == "" ? GenerateChestId() : overrideId;
        ChestInfo chestInfo = new ChestInfo(chestId, chestType);
        _chests.Add(chestId, chestInfo);
        return chestId;
    }

    private string GenerateChestId() {
        string chestId = $"{ChestIdPrefix}{StringGenerator.RandomString(ChestIdLength)}";
        return chestId;
    }

    private void OnNPCSpawned(string enemyId, NPCBehaviour npc) {
        if (_enemies.ContainsKey(enemyId)) {
            return;
        }
        LootInfo lootInfo = npc.Blueprint.LootInfo;
        if(lootInfo == null) {
            return;
        }
        int healthOrbCount = Random.Range(lootInfo.HealthOrbRange.Min, lootInfo.HealthOrbRange.Max);
        int manaOrbCount = Random.Range(lootInfo.ManaOrbRange.Min, lootInfo.ManaOrbRange.Max);

        List<string> inventoryItems = new List<string>();
        int totalTieredLootCount = MinMax_Int.GetCountFromMinMax(lootInfo.TieredLootRange);
        int currentTieredLootCount = 0;
        currentTieredLootCount =
            GenerateLootForTier(currentTieredLootCount, totalTieredLootCount, inventoryItems, lootInfo.WhiteTierLootRange, _lootTable[LootTier.White]);
        currentTieredLootCount =
            GenerateLootForTier(currentTieredLootCount, totalTieredLootCount, inventoryItems, lootInfo.GreenTierLootRange, _lootTable[LootTier.Green]);
        currentTieredLootCount =
            GenerateLootForTier(currentTieredLootCount, totalTieredLootCount, inventoryItems, lootInfo.PurpleTierLootRange, _lootTable[LootTier.Purple]);
        currentTieredLootCount =
            GenerateLootForTier(currentTieredLootCount, totalTieredLootCount, inventoryItems, lootInfo.GoldTierLootRange, _lootTable[LootTier.Gold]);

        // SUPER HACKY FIX FOR EDGE CASE
        while (currentTieredLootCount < totalTieredLootCount) {
            currentTieredLootCount++;
            inventoryItems.Add(GetRandomLootFromList(_lootTable[LootTier.White]));
        }

        RewardsSet rewards = new RewardsSet(healthOrbCount, manaOrbCount, inventoryItems);
        _enemies.Add(enemyId, rewards);
    }

    private int GenerateLootForTier(int currentCount, int maxCount, List<string> inventoryItems, MinMax_Int lootTierRange, List<string> lootTier) {
        int tieredLootCount = MinMax_Int.GetCountFromMinMax(lootTierRange);
        if (MaxLootCountReached(currentCount, tieredLootCount, maxCount)) {
            tieredLootCount = CapLootCount(maxCount, currentCount);
        }
        currentCount += tieredLootCount;
        AddToLootList(inventoryItems, lootTier, tieredLootCount);
        return currentCount;
    }

    private bool MaxLootCountReached(int currentCount, int nextAmount, int maxCount) {
        return currentCount + nextAmount > maxCount;
    }

    private int CapLootCount(int totalLootCount, int currentLootCount) {
        return Mathf.Max(totalLootCount - currentLootCount, 0);
    }

    private string GetRandomLootFromList(List<string> lootList) {
        int rand = Random.Range(0, lootList.Count);
        return lootList[rand];
    }

    private void AddToLootList(List<string> inventoryItems, List<string> lootTier, int count) {
        for(int i = 0; i < count; i++) {
            inventoryItems.Add(GetRandomLootFromList(lootTier));
        }
    }
}
