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
    [SerializeField] private MinMax_Int _healthOrbReward;
    [SerializeField] private MinMax_Int _manaOrbReward;
    [SerializeField] private MinMax_Int _whiteTierRange;
    [SerializeField] private MinMax_Int _greenTierRange;
    [SerializeField] private MinMax_Int _purpleTierRange;
    [SerializeField] private MinMax_Int _goldTierRange;

    public ChestType ChestType => _chestType;
    public MinMax_Int HealthOrbs => _healthOrbReward;
    public MinMax_Int ManaOrbs => _manaOrbReward;
    public MinMax_Int WhiteTier => _whiteTierRange;
    public MinMax_Int GreenTier => _greenTierRange;
    public MinMax_Int PurpleTier => _purpleTierRange;
    public MinMax_Int GoldTier => _purpleTierRange;
}
