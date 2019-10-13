using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ArenaConfig")]
public class ArenaLevelConfig : ScriptableObject {

    [SerializeField] private List<ArenaEnemyConfig> _enemyConfigs = new List<ArenaEnemyConfig>();
    public List<ArenaEnemyConfig> EnemyConfigs => _enemyConfigs;

    private readonly ArenaEnemyConfigComparer _enemyConfigComparer = new ArenaEnemyConfigComparer();

    [Range(1, 100)][SerializeField] private int _base;
    [Range(1f, 20f)][SerializeField] private float _multiplier;
    [Range(1, 100)] [SerializeField] private int _constant;

    [SerializeField] private List<string> _possibleLootIds = new List<string>();
    public IReadOnlyList<string> PossibleLootIds => _possibleLootIds;

    public List<ArenaEnemyCount> GetEnemyCountsForLevel(int level) {
        List<ArenaEnemyCount> enemyCounts = new List<ArenaEnemyCount>();
        // figure out enemy scaling
        return enemyCounts;
    }

    public List<string> GetEnemyQueue(int level) {
        _enemyConfigs.Sort(_enemyConfigComparer);
        List<string> enemyIdQueue = new List<string>();
        for(int i = 0; i < _enemyConfigs.Count; i++) {
            int count = _enemyConfigs[i].GetEnemyCount(level);
            for(int j = 0; j < count; j++) {
                enemyIdQueue.Add(_enemyConfigs[i].EnemyPrefabId);
            }
        }
        return enemyIdQueue;
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
    [SerializeField] private int _thresholdLevel;
    [SerializeField] private int _priority; // smaller int goes first

    public string EnemyPrefabId => _enemyPrefabId;
    public int LowCount => _lowCount;
    public int HighCount => _highCount;
    public int ThresholdLevel => _thresholdLevel;
    public int Priority => _priority;

    public int GetEnemyCount(int level) {
        if(level < _thresholdLevel) {
            return 0;
        }
        return Random.Range(GetMinimumCount(level), _highCount);
    }

    private int GetMinimumCount(int level) {
        return _lowCount + level;
    }
}

public class ArenaEnemyConfigComparer : IComparer<ArenaEnemyConfig> {

    public int Compare(ArenaEnemyConfig x, ArenaEnemyConfig y) {
        return x.ThresholdLevel.CompareTo(y.ThresholdLevel);
    }
}

public class ArenaEnemyCount {
    public readonly string EnemyPrefabId;
    public readonly int Count;
    public readonly int Priority;
}
