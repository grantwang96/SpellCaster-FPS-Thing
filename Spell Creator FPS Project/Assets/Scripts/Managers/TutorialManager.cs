using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour {

    public static TutorialManager Instance { get; private set; }

    private Queue<TutorialSet> _tutorialQueue = new Queue<TutorialSet>();
    private TutorialSet _currentTutorial;
    private int _tutorialActionIndex;

    public delegate void TutorialQueuedUpdated();
    public event TutorialQueuedUpdated OnTutorialQueueUpdated;

    private void Awake() {
        
    }

    public void TriggerTutorial(TutorialSet tutorial) {
        if (tutorial.ShouldTrigger()) {
            _tutorialQueue.Enqueue(tutorial);
        }
    }

    // hook this function into tutorial actions when they are completed
    private void TutorialActionCompleted() {
        if (_currentTutorial.HasNextTutorialAction(_tutorialActionIndex)) {
            _tutorialActionIndex++;
        }
        TutorialAction nextAction = _currentTutorial?.GetNextTutorialAction(_tutorialActionIndex);
        // do thing with action
    }
}