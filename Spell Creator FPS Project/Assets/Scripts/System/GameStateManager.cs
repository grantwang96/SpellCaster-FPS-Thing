﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour {

    public static GameStateManager Instance { get; private set; }
    public GameState CurrentState;
    public GameState PreviousState { get; private set; }

    private bool _initialized = false;

    [SerializeField] private GameState _bootState; // where will the game start on load

    private void Awake() {
        if (_initialized) { return; }
        if(Instance != null && Instance != this) {
            Debug.LogError($"[{nameof(GameStateManager)}] Game State Manager already exists!");
            return;
        }
        Instance = this;
    }

    // Use this for initialization
    void Start () {
        if (_initialized) { return; }
        Debug.Log($"[{nameof(GameStateManager)}] is loaded!");
        OnAppStart();
	}

    // this is when the game initially loads. Should be on loading screen here
    private void OnAppStart() {
        ChangeState(_bootState);
        _initialized = true;
    }

    public void HandleTransition(string transitionName) {
        if(CurrentState == null) {
            Debug.LogError($"[{nameof(GameStateManager)}] Current State is null! Wait what?");
            return;
        }
        GameState nextState = CurrentState.GetGameStateByTransitionName(transitionName);
        if(nextState == null) {
            Debug.Log($"[{nameof(GameStateManager)}] Could not retrieve game state for transition name {transitionName}");
            return;
        }
        ChangeState(nextState);
        /*
        for(int i = 0; i < CurrentState.Transitions.Count; i++) {
            if (CurrentState.Transitions[i].TransitionName.Equals(transitionName)) {
                GameStateTransition transition = CurrentState.Transitions[i];
                ChangeState(transition.GameState);
                break;
            }
        }*/
    }

    private void ChangeState(GameState newState) {
        if(CurrentState != null) {
            PreviousState = CurrentState;
            CurrentState.Exit(newState);
        }
        CurrentState = newState;
        CurrentState.Enter();
    }
}
