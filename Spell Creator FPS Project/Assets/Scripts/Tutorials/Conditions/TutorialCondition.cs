using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TutorialCondition : ScriptableObject {

    public virtual bool CanStartTutorial() {
        return true;
    }
}