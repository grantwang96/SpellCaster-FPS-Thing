using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaLevelConfig : ScriptableObject {

    [SerializeField] private List<ArenaEnemyConfig> _enemyConfigs = new List<ArenaEnemyConfig>();

    [Range(1, 100)][SerializeField] private int _base;
    [Range(1f, 20f)][SerializeField] private float _multiplier;
    [Range(1, 100)] [SerializeField] private int _constant;

    public List<ArenaEnemyConfig> GetEnemyConfigsForLevel(int level) {
        List<ArenaEnemyConfig> enemyConfigs = new List<ArenaEnemyConfig>();
        // figure out enemy scaling
        return enemyConfigs;
    }

    private int GetTotalEnemyCountForLevel(int level) {
        return (int)(_multiplier * Mathf.Log(level, _base) + _constant);
    }
}

[System.Serializable]
public class ArenaEnemyConfig {
    [SerializeField] private string _enemyPrefabId;
    [SerializeField] private int _lowCount;
    [SerializeField] private int _highCount;
    [SerializeField] private int _preferredLevel;
    [SerializeField] private int _levelLimitBelow;
    [SerializeField] private int _levelLimitAbove;
    [SerializeField] private int _priority; // smaller int goes first

    public string EnemyPrefabId => _enemyPrefabId;
    public int LowCount => _lowCount;
    public int HighCount => _highCount;
    public int PreferredLevel => _preferredLevel;
    public int LevelLimitBelow => _levelLimitBelow;
    public int LevelLimitAbove => _levelLimitAbove;
    public int Priority;
}
