using UnityEngine;

/// <summary>
/// Status that the tutorial action should return when action code is finished executing
/// </summary>
public enum TutorialActionStatus {
    Invalid,
    Abort,
    Incomplete,
    Complete,
}

public abstract class TutorialAction : ScriptableObject{

    public delegate void TutorialActionEvent();
    public event TutorialActionEvent OnTutorialActionComplete;

    public virtual TutorialActionStatus Execute() {
        return TutorialActionStatus.Complete;
    }

    protected virtual void TutorialActionCompleted() {
        OnTutorialActionComplete?.Invoke();
    }

    public virtual void Abort() {

    }
}