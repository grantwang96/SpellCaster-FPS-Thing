using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger_StateEntry : MonoBehaviour {

    [SerializeField] private string _triggerId;
    [SerializeField] private GameState _gameState;

    private void Awake() {
        if (_gameState == null) {
            _gameState = GetComponent<GameState>();
        }
    }

    // Use this for initialization
    void Start () {
        _gameState.OnGameStateEnter += OnGameStateEnter;
	}

    private void OnGameStateEnter() {
        TutorialManager.Instance.FireTutorialTrigger(_triggerId);
    }
}
