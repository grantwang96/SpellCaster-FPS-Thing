using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInfo {

    public readonly string ChestId;
    public readonly ChestType ChestType;

	public ChestInfo(string chestId, ChestType chestType) {
        ChestId = chestId;
        ChestType = chestType;
    }
}

public class RewardsSet {
    public readonly int HealthOrbs;
    public readonly int ManaOrbs;
    public readonly List<string> InventoryItems;

    public static RewardsSet Default = new RewardsSet(0, 0, new List<string>());

    public RewardsSet(int healthOrbs, int manaOrbs, List<string> inventoryItems) {
        HealthOrbs = healthOrbs;
        ManaOrbs = manaOrbs;
        InventoryItems = inventoryItems;
    }
}

[System.Serializable]
public class ChestTypeData {
    [SerializeField] private ChestType _chestType;
    [SerializeField] private MinMax _healthOrbReward;
    [SerializeField] private MinMax _manaOrbReward;
    [SerializeField] private MinMax _whiteTierRange;
    [SerializeField] private MinMax _greenTierRange;
    [SerializeField] private MinMax _purpleTierRange;
    [SerializeField] private MinMax _goldTierRange;

    public ChestType ChestType => _chestType;
    public MinMax HealthOrbs => _healthOrbReward;
    public MinMax ManaOrbs => _manaOrbReward;
    public MinMax WhiteTier => _whiteTierRange;
    public MinMax GreenTier => _greenTierRange;
    public MinMax PurpleTier => _purpleTierRange;
    public MinMax GoldTier => _purpleTierRange;
}
