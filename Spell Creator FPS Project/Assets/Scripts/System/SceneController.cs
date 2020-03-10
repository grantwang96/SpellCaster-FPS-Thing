using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SceneController : MonoBehaviour {

    private const string LoadingScreenScene = "LoadingScreen";

	public static SceneController Instance { get; private set; }

    public string CurrentSceneName => SceneManager.GetActiveScene().name;

    public event Action<string> OnSceneStartedLoading;
    public event Action<string> OnSceneFinishedLoading;

    private Action _onLoadingScreenAnimationCompleted;

    private string _nextSceneName;
    private bool isLoadingScene;

    private void Awake() {
        Instance = this;
    }

    public bool TransitionToScene(string sceneName) {
        /*
        if (CurrentSceneName.Equals(sceneName)) {
            FinishLoadingScene();
            return true;
        }
        */
        if (isLoadingScene) {
            return false;
        }
        _nextSceneName = sceneName;
        // check to see if we're already in that scene or if we're currently loading
        // play some transition animation
        _onLoadingScreenAnimationCompleted = OnEnterSceneTransition;
        FullScreenOverlayController.Instance.DisplayLoadingScreen();
        FullScreenOverlayController.Instance.LoadingScreenAnimationUpdated += OnScreenAnimationFinished;
        return true;
    }

    private void OnScreenAnimationFinished(AnimationState state) {
        switch (state) {
            case AnimationState.Completed:
                FullScreenOverlayController.Instance.LoadingScreenAnimationUpdated -= OnScreenAnimationFinished;
                _onLoadingScreenAnimationCompleted?.Invoke();
                _onLoadingScreenAnimationCompleted = null;
                break;
        }
    }

    // when transition animation ends -> should be in loading screen
    private void OnEnterSceneTransition() {
        OnSceneStartedLoading?.Invoke(_nextSceneName);
        LoadSceneInstant(LoadingScreenScene);
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_nextSceneName);
        isLoadingScene = true;
        while (!asyncLoad.isDone) {
            yield return new WaitForEndOfFrame();
        }
        FullScreenOverlayController.Instance.HideLoadingScreen();
        FinishLoadingScene();
    }

    private void FinishLoadingScene() {
        isLoadingScene = false;
        OnSceneFinishedLoading?.Invoke(_nextSceneName);
    }

    public bool LoadSceneInstant(string sceneName) {
        if(SceneManager.GetSceneByName(sceneName) == null) {
            return false;
        }
        SceneManager.LoadScene(sceneName);
        return true;
    }
}
