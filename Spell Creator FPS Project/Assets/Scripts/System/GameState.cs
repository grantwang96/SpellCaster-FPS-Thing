using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that handles processing the current game scenario
/// </summary>
public class GameState : MonoBehaviour {

    [SerializeField] private GameStateTransition[] _transitions; // list of transitions this can go to
    public GameStateTransition[] Transitions => _transitions;
    [SerializeField] private string _sceneName;
    
    public GameState ParentState { get; protected set; }
    private List<GameState> _childStates = new List<GameState>();

    public delegate void GameStateEvent();
    public event GameStateEvent OnGameStateEnter;

    private void Awake() {
        LoadParentState();
        LoadChildStates();
    }

    private void LoadParentState() {
        GameState parent = GetComponentInParent<GameState>();
        if(parent != null) {
            ParentState = parent;
        }
    }

    private void LoadChildStates() {
        GameState[] childStates = GetComponentsInChildren<GameState>();
        if (childStates != null && childStates.Length != 0) {
            _childStates = new List<GameState>(childStates);
        }
    }

    public GameState GetGameStateByTransitionName(string transitionName) {
        for(int i = 0; i < _transitions.Length; i++) {
            if (transitionName.Equals(_transitions[i].TransitionName)) {
                return _transitions[i].GameState;
            }
        }
        return null;
    }

    public virtual void Enter() {
        if (RequiresScene()) {
            TryToLoadScene();
        } else {
            FireGameStateEnteredEvent();
        }
    }

    private void TryToLoadScene() {
        bool success = SceneController.Instance.TransitionToScene(_sceneName);
        if (!success) {
            Debug.LogError("Failed to find and load scene!");
            return;
        }
        SceneController.Instance.OnSceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(string sceneName) {
        if (sceneName.Equals(_sceneName)) {
            SceneController.Instance.OnSceneLoaded -= OnSceneLoaded;
            FireGameStateEnteredEvent();
        }
    }

    protected void FireGameStateEnteredEvent() {
        Debug.Log($"Finished loading state {this.name}!");
        OnGameStateEnter?.Invoke();
    }

    protected bool RequiresScene() {
        return !string.IsNullOrEmpty(_sceneName);
    }

    public virtual void Exit() {

    }
}

[System.Serializable]
public class GameStateTransition {
    [SerializeField] private string _transitionName;
    [SerializeField] private GameState _gameState;

    public readonly string TransitionName;
    public readonly GameState GameState;
}
