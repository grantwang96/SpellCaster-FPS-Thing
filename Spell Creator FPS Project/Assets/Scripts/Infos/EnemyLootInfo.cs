using System.Collections.Generic;
using UnityEngine;

public class EnemyLootData {

    public readonly string EnemyId;
    public readonly int HealthOrbCount;
    public readonly int ManaOrbCount;
    public readonly List<string> RuneIds;

    public EnemyLootData(string enemyId, int healthOrbCount, int manaOrbCount, List<string> runeIds) {
        EnemyId = enemyId;
        HealthOrbCount = healthOrbCount;
        ManaOrbCount = manaOrbCount;
        RuneIds = runeIds;
    }
}

[System.Serializable]
public class LootInfo {
    [SerializeField] private MinMax_Int _healthOrbRange;
    [SerializeField] private MinMax_Int _manaOrbRange;
    [SerializeField] private MinMax_Int _tieredLootRange;
    [SerializeField] private MinMax_Int _whiteTierLootRange;
    [SerializeField] private MinMax_Int _greenTierLootRange;
    [SerializeField] private MinMax_Int _purpleTierLootRange;
    [SerializeField] private MinMax_Int _goldTierLootRange;

    public MinMax_Int HealthOrbRange => _healthOrbRange;
    public MinMax_Int ManaOrbRange => _manaOrbRange;
    public MinMax_Int TieredLootRange => _tieredLootRange;
    public MinMax_Int WhiteTierLootRange => _whiteTierLootRange;
    public MinMax_Int GreenTierLootRange => _greenTierLootRange;
    public MinMax_Int PurpleTierLootRange => _purpleTierLootRange;
    public MinMax_Int GoldTierLootRange => _goldTierLootRange;
}
