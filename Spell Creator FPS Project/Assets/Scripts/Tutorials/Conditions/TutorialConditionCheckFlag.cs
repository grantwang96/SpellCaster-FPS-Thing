using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorials/Condition/Check Flag")]
public class TutorialConditionCheckFlag : TutorialCondition {

    [SerializeField] private string _flagName;
    [SerializeField] private bool _value;

    public override bool CanStartTutorial() {
        return PlayerDataManager.Instance.GetFlag(_flagName) == _value;
    }
}