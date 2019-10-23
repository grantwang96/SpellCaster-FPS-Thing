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
    [SerializeField] [Range(1, 1000)] private int _dropWeight;

    public string LootId => _lootId;
    public int DropWeight => _dropWeight;

    // when code wants to use this
    public LootInfo(string lootId, int dropWeight) {
        _lootId = lootId;
        _dropWeight = dropWeight;
    }
} 
