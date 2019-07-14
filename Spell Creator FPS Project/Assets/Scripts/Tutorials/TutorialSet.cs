using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A serializeable object containing all the data for a tutorial
/// </summary>
[CreateAssetMenu(menuName = "Tutorials/Tutorial Set")]
public class TutorialSet : ScriptableObject {

    [SerializeField] private string _tutorialId; // the tutorial id to reference and set flags for
    public string TutorialId => _tutorialId;
    [SerializeField] private bool _repeatable;

    [SerializeField] private List<TutorialCondition> _conditions = new List<TutorialCondition>();
    [SerializeField] private List<TutorialAction> _actions = new List<TutorialAction>();

    public delegate void TutorialEntryEvent();
    public event TutorialEntryEvent OnTutorialEntered;

    public virtual bool ShouldTrigger() {
        bool shouldTrigger = !IsTutorialTriggered();
        // check conditions here
        for(int i = 0; i < _conditions.Count; i++) {
            shouldTrigger &= _conditions[i].CanStartTutorial();
        }
        return shouldTrigger;
    }

    protected bool IsTutorialTriggered() {
        return PlayerDataManager.Instance.GetFlag(_tutorialId);
    }

    public virtual void EnterTutorial() {
        OnTutorialEntered?.Invoke();
    }

    public bool HasNextTutorialAction(int index) {
        return index < _actions.Count - 1;
    }

    public virtual TutorialAction GetTutorialAction(int index) {
        if(index < 0 || index >= _actions.Count) {
            Debug.LogError($"[{name}] Tutorial Action index {index} out of range!");
            return null;
        }
        return _actions[index];
    }

    public virtual void ExitTutorial() {
        if (_repeatable) {
            return;
        }
        PlayerDataManager.Instance.SetFlag(_tutorialId, true);
    }
}
