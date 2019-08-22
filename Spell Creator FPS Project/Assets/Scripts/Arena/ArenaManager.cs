using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void RoundStateUpdateDelegate(int round);
public delegate void ArenaStatDelegate(int count);

public interface IArenaManager {
    void StartRound();

    event RoundStateUpdateDelegate OnRoundStarted;
    event RoundStateUpdateDelegate OnRoundEnded;
    
    event ArenaStatDelegate OnEnemyCountUpdated;
    event ArenaStatDelegate OnWaveCountUpdated;
}

public class ArenaManager : MonoBehaviour, IArenaManager {

    private const string EnemiesResourcePath = "Enemies";

    public static IArenaManager Instance;

    [SerializeField] private ArenaLevelConfig _config;
    public int CurrentLevel { get; private set; }
    private bool _currentlyRunningRound;
    private int _enemyCount;
    private int _enemiesDefeated;
    private int _currentWaveCount;
    private List<string> _nextRound = new List<string>();
    private List<string> _nextWave = new List<string>();
    private List<Damageable> _currentWave = new List<Damageable>();

    [SerializeField] private List<Transform> _spawnPoints;

    public event RoundStateUpdateDelegate OnRoundStarted;
    public event RoundStateUpdateDelegate OnRoundEnded;
    
    public event ArenaStatDelegate OnEnemyCountUpdated;
    public event ArenaStatDelegate OnWaveCountUpdated;
    
    private void Awake() {
        Instance = this;
    }

    private void Start() {
        // temp
        Initialize(_config);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.K) && !_currentlyRunningRound) {
            StartRound();
        }
    }

    public void Initialize(ArenaLevelConfig config) {
        _config = config;
        GameplayController.Instance.Damageable.OnDeath += LoseRound;
        RegisterEnemyPrefabs();
    }

    private void RegisterEnemyPrefabs() {
        for (int i = 0; i < _config.EnemyConfigs.Count; i++) {
            ObjectPool.Instance.RegisterObjectToPool(EnemiesResourcePath, _config.EnemyConfigs[i].EnemyPrefabId, _config.EnemyConfigs[i].HighCount);
        }
    }

    // initiate the next round
    public void StartRound() {
        _enemiesDefeated = 0;
        _currentlyRunningRound = true;
        _currentWaveCount = 0;
        CurrentLevel++;
        Debug.Log($"Starting Round {CurrentLevel}");
        GenerateNextRound();
        GenerateNextWave();
        OnRoundStarted?.Invoke(CurrentLevel);
    }

    // check if next wave needs to be spawned or if round is over here
    private void OnEnemyDefeated(bool isDead, Damageable damageable) {
        _enemiesDefeated++;
        damageable.OnDeath -= OnEnemyDefeated;
        _currentWave.Remove(damageable);
        if(_enemiesDefeated == _enemyCount) {
            WinRound();
            return;
        }
        if(_currentWave.Count == 0 && _nextWave.Count == 0) {
            GenerateNextWave();
        }
    }

    // actually ending the round here
    private void WinRound() {
        if (!_currentlyRunningRound) {
            return;
        }
        _currentlyRunningRound = false;
        _nextRound.Clear();
        OnRoundEnded?.Invoke(CurrentLevel);
        Debug.Log($"Round {CurrentLevel} complete!");
    }

    private void LoseRound(bool isDead, Damageable damageable) {
        _currentlyRunningRound = false;
        // handle losing screen here
        Debug.Log($"Round {CurrentLevel} lost!");
    }

    // initiates the next round
    private void GenerateNextRound() {
        // retrieve list of enemy configurations from config object
        _nextRound = _config.GetEnemyQueue(CurrentLevel);
        OnEnemyCountUpdated?.Invoke(_nextRound.Count);
        _enemyCount = _nextRound.Count;
    }

    // initiates the next wave
    private void GenerateNextWave() {
        _currentWaveCount++;
        int waveCount = GetWaveCount();
        if(waveCount > _nextRound.Count) {
            waveCount = _nextRound.Count;
        }
        OnWaveCountUpdated?.Invoke(waveCount);
        _currentWave.Clear();
        _nextWave = _nextRound.GetRange(0, waveCount);
        Debug.Log("Wave Size: " + _nextWave.Count);
        for(int i = 0; i < _nextWave.Count; i++) {
            _nextRound.Remove(_nextWave[i]);
        }
        StartCoroutine(SpawnWave(_nextWave));
    }

    private IEnumerator SpawnWave(List<string> enemyPrefabIds) {
        for (int i = 0; i < enemyPrefabIds.Count; i++) {
            yield return new WaitForSeconds(0.5f);
            int rand = Random.Range(0, _spawnPoints.Count);
            Transform spawnPoint = _spawnPoints[rand];
            SpawnEnemyPrefab(enemyPrefabIds[i], spawnPoint.position);
        }
        _nextWave.Clear();
        Debug.Log("Finished Spawning wave!");
    }

    private void SpawnEnemyPrefab(string prefabName, Vector3 position) {
        Debug.Log("Prefab Name: " + prefabName);
        PooledObject pooledObject = ObjectPool.Instance.UsePooledObject(prefabName);
        EnemyBehaviour enemy = pooledObject as EnemyBehaviour;
        if(enemy == null) {
            ErrorManager.LogError(nameof(ArenaManager), $"Pooled object {pooledObject} was not of type {nameof(EnemyBehaviour)}");
            return;
        }
        enemy.transform.position = position;
        enemy.Damageable.OnDeath += OnEnemyDefeated;
        _currentWave.Add(enemy.Damageable);
        enemy.ActivatePooledObject();
        Debug.Log("Spawned Enemy Prefab!");
    }

    private int GetWaveCount() {
        return Random.Range(WaveCountMin(), WaveCountMax());
    }

    private int WaveCountMin() {
        return CurrentLevel + CurrentLevel / 2;
    }

    private int WaveCountMax() {
        return CurrentLevel + CurrentLevel * 2;
    }
}
