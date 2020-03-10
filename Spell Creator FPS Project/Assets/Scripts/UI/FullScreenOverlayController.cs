using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FullScreenOverlayController : MonoBehaviour
{
    [SerializeField] private Animator _blackScreen;
    [SerializeField] private Animator _loadingScreenObject;
    [SerializeField] private AnimationStateBroadcaster _blackScreenAnimationBroadcaster;
    [SerializeField] private AnimationStateBroadcaster _loadingScreenAnimationBroadcaster;

    public event Action<AnimationState> BlackScreenAnimationUpdated;
    public event Action<AnimationState> LoadingScreenAnimationUpdated;

    public static FullScreenOverlayController Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        // SceneController.Instance.OnSceneFinishedLoading += OnSceneLoaded;
        _blackScreenAnimationBroadcaster.OnAnimationStateUpdated += OnBlackScreenUpdated;
        _loadingScreenAnimationBroadcaster.OnAnimationStateUpdated += OnLoadingScreenUpdated;
    }

    private void OnDestroy() {
        // SceneController.Instance.OnSceneFinishedLoading -= OnSceneLoaded;
        _blackScreenAnimationBroadcaster.OnAnimationStateUpdated -= OnBlackScreenUpdated;
        _loadingScreenAnimationBroadcaster.OnAnimationStateUpdated -= OnLoadingScreenUpdated;
    }

    private void OnSceneLoaded(string sceneName) {
        // HideLoadingScreen();
    }

    public void ShowBlackScreen() {
        _blackScreen.Play("Show");
    }

    public void HideBlackScreen() {
        _blackScreen.Play("Hide");
    }

    public void DisplayLoadingScreen() {
        _loadingScreenObject.Play("Show");
    }

    public void HideLoadingScreen() {
        _loadingScreenObject.Play("Hide");
    }

    private void OnBlackScreenUpdated(AnimationState state) {
        BlackScreenAnimationUpdated?.Invoke(state);
    }

    private void OnLoadingScreenUpdated(AnimationState state) {
        LoadingScreenAnimationUpdated?.Invoke(state);
    }
}
