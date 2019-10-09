using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that handles processing the current game scenario
/// </summary>
public class GameState : MonoBehaviour {

    [SerializeField] private List<GameStateTransition> _transitions = new List<GameStateTransition>(); // list of transitions this can go to
    public IReadOnlyList<GameStateTransition> Transitions => _transitions;
    [SerializeField] private string _sceneName;

    private bool _initialized = false;

    public bool IsLoading { get; protected set; }
    public bool IsActive;
    public GameState ParentState { get; protected set; }
    private List<GameState> _childStates = new List<GameState>();

    public delegate void GameStateEvent();
    public event GameStateEvent OnGameStateEnter;
    public event GameStateEvent OnGameStateExit;

    private void Awake() {
        if (_initialized) { return; }
        LoadParentState();
        LoadChildStates();
        _initialized = true;
    }

    private void LoadParentState() {
        if(transform.parent == null) {
            return;
        }
        GameState parentState = transform.parent.GetComponent<GameState>();
        if(parentState != null) {
            ParentState = parentState;
        }
    }

    private void LoadChildStates() {
        GameState[] childStates = GetComponentsInChildren<GameState>();
        if (childStates != null && childStates.Length != 0) {
            _childStates = new List<GameState>(childStates);
        }
    }

    public GameState GetGameStateByTransitionName(string transitionName) {
        for(int i = 0; i < _transitions.Count; i++) {
            if (transitionName.Equals(_transitions[i].TransitionName)) {
                return _transitions[i].GameState;
            }
        }
        if(ParentState != null) {
            return ParentState.GetGameStateByTransitionName(transitionName);
        }
        return null;
    }

    public virtual void Enter() {
        IsLoading = true;
        if (ParentState != null) {
            ParentState.Enter();
            ParentState.OnGameStateEnter += OnReadyToEnter;
            return;
        }
        OnReadyToEnter();
    }

    protected void OnReadyToEnter() {
        if (RequiresScene()) {
            // that means a scene transition is necessary and we haven't fully loaded yet
            TryToLoadScene();
            return;
        }
        OnStateLoaded();
    }

    private void TryToLoadScene() {
        bool success = SceneController.Instance.TransitionToScene(_sceneName);
        if (!success) {
            Debug.LogError($"[{this.name}][{nameof(GameState)}] Failed to find and load scene!");
            return;
        }
        SceneController.Instance.OnSceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(string sceneName) {
        if (sceneName.Equals(_sceneName)) {
            SceneController.Instance.OnSceneLoaded -= OnSceneLoaded;
            OnStateLoaded();
        }
    }

    // when the game the state has finished loading everything
    protected void OnStateLoaded() {
        IsLoading = false;
        IsActive = true;
        Debug.Log($"Entered game state {this.name}!");
        OnGameStateEnter?.Invoke();
        if(ParentState != null) {
            ParentState.OnGameStateEnter -= OnStateLoaded;
        }
    }

    protected bool RequiresScene() {
        return !string.IsNullOrEmpty(_sceneName);
    }

    public virtual void Exit(GameState nextState) {
        // check to see if we actually need to exit
        if (nextState.StateOnPath(this)) {
            return;
        }
        IsActive = false;
        OnGameStateExit?.Invoke();
    }

    // check if a given state is on this state's active path
    public bool StateOnPath(GameState state) {
        if(ParentState == null) {
            return false;
        }
        if(this == state) {
            return true;
        }
        return StateOnPath(state);
    }
}

[System.Serializable]
public class GameStateTransition {
    [SerializeField] private string _transitionName;
    [SerializeField] private GameState _gameState;

    public string TransitionName => _transitionName;
    public GameState GameState => _gameState;
}
