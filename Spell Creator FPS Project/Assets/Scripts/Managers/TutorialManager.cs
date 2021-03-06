﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour {

    private const string TutorialFolderBasePath = "Tutorials";
    private const string TutorialSetBaseName = "TutorialSet_";
    public static TutorialManager Instance { get; private set; }

    // [SerializeField] private string[] _tutorialFolderPaths;

    [SerializeField] private List<TutorialSet> _tutorialSets = new List<TutorialSet>();
    private Queue<TutorialSet> _tutorialQueue = new Queue<TutorialSet>();
    private TutorialSet _currentTutorial;
    [SerializeField] private int _tutorialActionIndex = 0;
    private TutorialAction _currentAction;

    public delegate void TutorialQueuedUpdated();
    public event TutorialQueuedUpdated OnTutorialQueueUpdated;

    private void Awake() {
        Instance = this;
        // LoadAllTutorials();
    }

    /*
    private void LoadAllTutorials() {
        for(int i = 0; i < _tutorialFolderPaths.Length; i++) {
            string tutorialPath = $"{TutorialFolderBasePath}/{_tutorialFolderPaths[i]}/{TutorialSetBaseName}{_tutorialFolderPaths[i]}";
            Object tutorialObject = Resources.Load(tutorialPath);
            if(tutorialObject == null) {
                Debug.LogError($"[{nameof(TutorialManager)}] Could not find tutorial set object for \"{_tutorialFolderPaths[i]}\"");
                continue;
            }
            TutorialSet tutorialSet = tutorialObject as TutorialSet;
            if(tutorialSet == null) {
                Debug.LogError($"[{nameof(TutorialManager)}] Object \"{_tutorialFolderPaths[i]}\" was not of type TutorialSet");
                continue;
            }
            _tutorialSets.Add(tutorialSet);
        }
    }
    */
    
    public void FireTutorialTrigger(string triggerId) {
        _tutorialQueue.Clear();
        for(int i = 0; i < _tutorialSets.Count; i++) {
            if (_tutorialSets[i].ShouldTrigger(triggerId)) {
                _tutorialQueue.Enqueue(_tutorialSets[i]);
            }
        }
        if(_tutorialQueue.Count == 0) {
            return;
        }
        _currentTutorial = _tutorialQueue.Dequeue();
        RunTutorial();
    }
    
    // function that gets next tutorial action and runs it
    private void RunTutorial() {
        if (_currentAction != null) { // do not run while another TutorialAction hasn't finished yet
            return;
        }
        if(_currentTutorial == null) {
            Debug.Log($"[{nameof(TutorialManager)}] Tutorial Queue was empty!");
            return;
        }
        RunNextTutorialAction();
    }

    private void RunNextTutorialAction() {
        _currentAction = _currentTutorial?.GetTutorialAction(_tutorialActionIndex);
        if(_currentAction == null) {
            TutorialCompleted();
            return;
        }
        TutorialActionStatus result = _currentAction.Execute();
        switch (result) {
            case TutorialActionStatus.Abort:
                // do not run this entire tutorial set
                AbortTutorial();
                break;
            case TutorialActionStatus.Incomplete:
                // subscribe to the on complete action
                _currentAction.OnTutorialActionComplete += TutorialActionCompleted;
                break;
            case TutorialActionStatus.Complete:
                // immediately go to the next tutorial action
                _tutorialActionIndex++;
                RunNextTutorialAction();
                break;
            default:
                Debug.LogError($"[{nameof(TutorialManager)}] Invalid execute action response from tutorial action {_currentAction.name}");
                break;
        }
    }

    // hook this function into tutorial actions when they are completed
    private void TutorialActionCompleted() {
        _currentAction.OnTutorialActionComplete -= TutorialActionCompleted;
        _currentAction = null;
        if (!_currentTutorial.HasNextTutorialAction(_tutorialActionIndex)) {
            // tutorial is complete and we can move on to next tutorial in queue
            TutorialCompleted();
            // if there are no queued tutorials then return
            if (_tutorialQueue.Count == 0) {
                return;
            }
            _currentTutorial = _tutorialQueue.Peek();
        } else {
            _tutorialActionIndex++;
        }
        // run the next tutorial action
        RunTutorial();
    }

    // when a tutorial is successfully completed
    private void TutorialCompleted() {
        _currentTutorial.ExitTutorial();
        ContinueTutorialQueue();
    }

    private void ContinueTutorialQueue() {
        _tutorialActionIndex = 0;
        if (_tutorialQueue.Count != 0) {
            _currentTutorial = _tutorialQueue.Dequeue();
            RunNextTutorialAction();
            return;
        }
        _currentTutorial = null;
    }

    private void AbortTutorial() {
        // abort all tutorial actions
        ErrorManager.LogError(nameof(TutorialManager), $"Aborting tutorial {_currentTutorial.TutorialId} at action index \"{_tutorialActionIndex}\"!");
        for(int i = 0; i < _tutorialActionIndex; i++) {
            TutorialAction action = _currentTutorial.GetTutorialAction(i);
            action?.Abort();
        }
        ContinueTutorialQueue();
    }
}