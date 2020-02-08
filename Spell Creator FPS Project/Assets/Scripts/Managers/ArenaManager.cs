using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public delegate void RoundStateUpdateDelegate(int round);
public delegate void ArenaStatDelegate(int count);

public interface IArenaManager {
    void StartRound();

    event RoundStateUpdateDelegate OnRoundStarted;
    event RoundStateUpdateDelegate OnRoundEnded;
    
    event ArenaStatDelegate OnEnemyCountUpdated;
    event ArenaStatDelegate OnWaveCountUpdated;

    event Action<int> OnEnemyDefeated;
}

public class ArenaManager : GameManager, IArenaManager {

    private const string EnemiesResourcePath = "Enemies";

    public static IArenaManager ArenaManagerInstance { get; private set; }

    [SerializeField] private ArenaLevelConfig _config;
    [SerializeField] private string _winScreenPrefabId;
    [SerializeField] private string _loseScreenPrefabId;

    private ArenaStats ArenaStats;

    public int CurrentLevel { get; private set; }
    private bool _currentlyRunningRound;
    private int _enemyCount;
    private int _enemiesDefeated;
    private int _currentWaveCount;
    private List<string> _nextRound = new List<string>();
    private List<string> _nextWave = new List<string>();

    private Dictionary<string, IRaycastInteractable> _interactables = new Dictionary<string, IRaycastInteractable>();
    private Dictionary<Damageable, NPCBehaviour> _currentWave = new Dictionary<Damageable, NPCBehaviour>();

    [SerializeField] private List<Transform> _spawnPoints;

    public event RoundStateUpdateDelegate OnRoundStarted;
    public event RoundStateUpdateDelegate OnRoundEnded;
    
    public event ArenaStatDelegate OnEnemyCountUpdated;
    public event ArenaStatDelegate OnWaveCountUpdated;

    public event Action<int> OnEnemyDefeated;
    
    protected override void Awake() {
        base.Awake();
        ArenaManagerInstance = this;
    }

    protected override void SetInventories() {
        TempInventory tempInventory = new TempInventory(true, PersistedInventory.RunicInventory, PersistedInventory.SpellInventory);
        CurrentRunicInventory = tempInventory;
        CurrentSpellInventory = tempInventory;
    }

    protected override void Start() {
        // temp
        base.Start();
        Initialize(_config);
    }

    private void Update() {
        // HackInputs();
    }

    private void HackInputs() {
        if (Input.GetKeyDown(KeyCode.O)) {
            GameStateManager.Instance.HandleTransition(GameplayValues.Navigation.EnterTutorialLevelTransitionId);
        }
    }

    public void Initialize(ArenaLevelConfig config) {
        _config = config;
        InitializeArenaStats();
        RegisterEnemyPrefabs();
        UIPanelManager.Instance.RegisterUIPanel(_loseScreenPrefabId);
        PlayerController.Instance.Damageable.OnDeath += LoseRound;
    }
    private void InitializeArenaStats() {
        ArenaStats = new ArenaStats();
    }

    private void OnDestroy() {
        UIPanelManager.Instance.DeregisterUIPanel(_loseScreenPrefabId);
        PlayerController.Instance.Damageable.OnDeath -= LoseRound;
    }

    private void RegisterEnemyPrefabs() {
        for (int i = 0; i < _config.EnemyConfigs.Count; i++) {
            PooledObjectManager.Instance.RegisterPooledObject(_config.EnemyConfigs[i].EnemyPrefabId, _config.EnemyConfigs[i].HighCount);
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
    private void OnEnemyDefeatedListener(bool isDead, Damageable damageable) {
        NPCBehaviour enemy;
        if(!_currentWave.TryGetValue(damageable, out enemy)) {
            return;
        }
        _enemiesDefeated++;
        damageable.OnDeath -= OnEnemyDefeatedListener;
        SendEnemyDefeatedMessage(enemy);
        _currentWave.Remove(damageable);

        OnEnemyCountUpdated?.Invoke(_enemyCount - _enemiesDefeated);
        OnWaveCountUpdated?.Invoke(_currentWave.Count);

        if (_enemiesDefeated == _enemyCount) {
            WinRound();
            OnWaveCountUpdated?.Invoke(_currentWave.Count);
            return;
        }
        if(_currentWave.Count == 0 && _nextWave.Count == 0) {
            GenerateNextWave();
        }
    }

    private void SendEnemyDefeatedMessage(NPCBehaviour enemy) {
        int scoreValue = enemy.Blueprint.ScoreValue;
        OnEnemyDefeated?.Invoke(scoreValue);
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
        UIPanelManager.Instance.OpenUIPanel(_loseScreenPrefabId,
            new ArenaLoseScreenInitData() {
                ArenaStats = ArenaStats
        });
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
        _currentWave.Clear();
        _nextWave = _nextRound.GetRange(0, waveCount);
        for (int i = 0; i < _nextWave.Count; i++) {
            _nextRound.Remove(_nextWave[i]);
        }
        StartCoroutine(SpawnWave(_nextWave));
    }

    private IEnumerator SpawnWave(List<string> enemyPrefabIds) {
        for (int i = 0; i < enemyPrefabIds.Count; i++) {
            yield return new WaitForSeconds(1f);
            int rand = UnityEngine.Random.Range(0, _spawnPoints.Count);
            Transform spawnPoint = _spawnPoints[rand];
            SpawnEnemyPrefab(enemyPrefabIds[i], spawnPoint.position);
        }
        _nextWave.Clear();
    }

    private void SpawnEnemyPrefab(string prefabName, Vector3 position) {
        Quaternion forward = Quaternion.LookRotation(-position);
        NPCBehaviour npc = NPCManager.Instance.SpawnPooledNPC(prefabName, position, forward.eulerAngles, string.Empty);
        npc.Damageable.OnDeath += OnEnemyDefeatedListener;
        _currentWave.Add(npc.Damageable, npc);
        OnWaveCountUpdated?.Invoke(_currentWave.Count);
    }

    private int GetWaveCount() {
        return UnityEngine.Random.Range(WaveCountMin(), WaveCountMax());
    }

    private int WaveCountMin() {
        return CurrentLevel + CurrentLevel / 2;
    }

    private int WaveCountMax() {
        return CurrentLevel + CurrentLevel * 2;
    }
}
