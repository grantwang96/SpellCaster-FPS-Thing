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
    [SerializeField] private string _lootId;
    [SerializeField] private float _chance;
    [SerializeField] private MinMax_Int _range;

    public string LootId => _lootId;
    public float Chance => _chance;
    public MinMax_Int Range => _range;
}
