using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IArenaManager {
    void StartRound();
}

public class ArenaManager : MonoBehaviour, IArenaManager {

    public static IArenaManager Instance;

    private ArenaLevelConfig _config;
    private int _currentLevel = 0;
    private int _enemyCount;
    private int _enemiesDefeated;
    private List<EnemyBehaviour> _enemiesToBeSpawned = new List<EnemyBehaviour>();
    
    private void Awake() {
        Instance = this;
    }

    public void Initialize(ArenaLevelConfig config) {
        _config = config;
    }

    // initiate the next round
    public void StartRound() {
        _enemiesDefeated = 0;
        _currentLevel++;
        GenerateNextWave();
    }

    private void OnEnemyDefeated(bool isDead) {
        _enemiesDefeated++;
    }

    private void EndRound() {
        _enemiesToBeSpawned.Clear();
    }

    private void GenerateNextWave() {
        List<ArenaEnemyConfig> enemyConfigs = _config.GetEnemyConfigsForLevel(_currentLevel);
    }
}
