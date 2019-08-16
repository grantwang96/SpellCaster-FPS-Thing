using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IArenaManager {
    void StartRound();
}

public class ArenaManager : MonoBehaviour, IArenaManager {

    private const string EnemiesResourcePath = "Enemies";

    public static IArenaManager Instance;

    [SerializeField] private ArenaLevelConfig _config;
    public int CurrentLevel { get; private set; }
    private bool _currentlyRunningRound;
    private int _enemyCount;
    private int _enemiesDefeated;
    private List<string> _nextRound = new List<string>();
    private List<string> _nextWave = new List<string>();
    private List<Damageable> _currentWave = new List<Damageable>();

    [SerializeField] private List<Transform> _spawnPoints;
    
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
        CurrentLevel++;
        GenerateNextRound();
        GenerateNextWave();
    }

    // check if next wave needs to be spawned or if round is over here
    private void OnEnemyDefeated(bool isDead, Damageable damageable) {
        _enemiesDefeated++;
        damageable.OnDeath -= OnEnemyDefeated;
        _currentWave.Remove(damageable);
        if(_enemiesDefeated == _enemyCount) {
            EndRound();
            return;
        }
        if(_currentWave.Count == 0 && _nextWave.Count == 0) {
            GenerateNextWave();
        }
    }

    // actually ending the round here
    private void EndRound() {
        _currentlyRunningRound = false;
        _nextRound.Clear();
    }

    // initiates the next round
    private void GenerateNextRound() {
        // retrieve list of enemy configurations from config object
        _nextRound = _config.GetEnemyQueue(CurrentLevel);
        _enemyCount = _nextRound.Count;
    }

    // initiates the next wave
    private void GenerateNextWave() {
        int waveCount = GetWaveCount();
        if(waveCount > _nextRound.Count) {
            waveCount = _nextRound.Count;
        }
        _currentWave.Clear();
        _nextWave = _nextRound.GetRange(0, waveCount);
        for(int i = 0; i < _nextWave.Count; i++) {
            _nextRound.Remove(_nextWave[i]);
        }
        SpawnWaveInstant(_nextWave);
        // StartCoroutine(SpawnWave(_nextWave));
    }

    private IEnumerator SpawnWave(List<string> enemyPrefabIds) {
        Transform[] spawnpoints = new Transform[_spawnPoints.Count];
        for(int i = 0; i < _spawnPoints.Count; i++) {
            spawnpoints[i] = _spawnPoints[i];
        }
        for (int i = 0; i < enemyPrefabIds.Count; i++) {
            yield return new WaitForSeconds(0.5f);
            int rand = Random.Range(0, spawnpoints.Length);
            Debug.Log("Array length: " + spawnpoints.Length);
            Debug.Log("Index: " + rand);
            Transform spawnPoint = spawnpoints[rand];
            SpawnEnemyPrefab(enemyPrefabIds[i], spawnPoint.position);
        }
        _nextWave.Clear();
    }

    private void SpawnWaveInstant(List<string> enemyPrefabIds) {
        for (int i = 0; i < enemyPrefabIds.Count; i++) {
            int rand = Random.Range(0, _spawnPoints.Count);
            Debug.Log("Array length: " + _spawnPoints.Count);
            Debug.Log("Index: " + rand);
            Transform spawnPoint = _spawnPoints[rand];
            SpawnEnemyPrefab(enemyPrefabIds[i], spawnPoint.position);
        }
    }

    private void SpawnEnemyPrefab(string prefabName, Vector3 position) {
        PooledObject pooledObject = ObjectPool.Instance.UsePooledObject(prefabName);
        EnemyBehaviour enemy = pooledObject as EnemyBehaviour;
        if(enemy == null) {
            ErrorManager.LogError(nameof(ArenaManager), $"Pooled object {pooledObject} was not of type {nameof(EnemyBehaviour)}");
            return;
        }
        enemy.transform.position = position;
        enemy.Damageable.OnDeath += OnEnemyDefeated;
        enemy.ActivatePooledObject();
    }

    private int GetWaveCount() {
        int count = CurrentLevel + Random.Range(Mathf.CeilToInt(CurrentLevel / 2f), CurrentLevel);
        return count;
    }
}
