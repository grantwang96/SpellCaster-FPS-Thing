using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArenaHud : MonoBehaviour {

    [SerializeField] private Text _totalEnemyCount;
    [SerializeField] private Text _currentWaveSize;
    [SerializeField] private Text _currentRound;

	// Use this for initialization
	void Start () {
        OnRoundStarted(0);
        OnEnemyCountUpdated(0);
        OnWaveCountUpdated(0);
		if(ArenaManager.ArenaManagerInstance != null) {
            ArenaManager.ArenaManagerInstance.OnRoundStarted += OnRoundStarted;
            ArenaManager.ArenaManagerInstance.OnEnemyCountUpdated += OnEnemyCountUpdated;
            ArenaManager.ArenaManagerInstance.OnWaveCountUpdated += OnWaveCountUpdated;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnRoundStarted(int round) {
        _currentRound.text = $"Round: {round}";
    }

    private void OnEnemyCountUpdated(int count) {
        _totalEnemyCount.text = $"Total Enemies: {count}";
    }

    private void OnWaveCountUpdated(int count) {
        _currentWaveSize.text = $"Current Wave: {count}";
    }
}
