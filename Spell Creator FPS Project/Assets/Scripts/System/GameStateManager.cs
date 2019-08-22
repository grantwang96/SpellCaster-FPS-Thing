using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour {

    public static GameStateManager Instance { get; private set; }
    public GameState CurrentState { get; private set; }
    public GameState PreviousState { get; private set; }

    [SerializeField] private GameState _bootState; // where will the game start on load
    [SerializeField] private GameState _startState; // where will that loading screen take us on app boot

    private void Awake() {
        if(Instance != null && Instance != this) {
            Debug.LogError($"[{nameof(GameStateManager)}] Game State Manager already exists!");
            return;
        }
        Instance = this;
    }

    // Use this for initialization
    void Start () {
        Debug.Log($"[{nameof(GameStateManager)}] is loaded!");
        OnAppStart();
	}

    // this is when the game initially loads. Should be on loading screen here
    private void OnAppStart() {
        ChangeState(_startState);
    }

    public void HandleTransition(string transitionName) {
        if(CurrentState == null) {
            Debug.LogError($"[{nameof(GameStateManager)}] Current State is null! Wait what?");
            return;
        }
        for(int i = 0; i < CurrentState.Transitions.Length; i++) {
            if (CurrentState.Transitions[i].TransitionName.Equals(transitionName)) {
                GameStateTransition transition = CurrentState.Transitions[i];
                ChangeState(transition.GameState);
                break;
            }
        }
    }

    private void ChangeState(GameState state) {
        if(CurrentState != null) {
            PreviousState = CurrentState;
            CurrentState.Exit();
        }
        CurrentState = state;
        CurrentState.Enter();
    }
}
