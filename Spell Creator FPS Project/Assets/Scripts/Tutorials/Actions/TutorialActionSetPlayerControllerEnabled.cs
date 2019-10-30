using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorials/Actions/SetPlayerControllerEnabled")]
public class TutorialActionSetPlayerControllerEnabled : TutorialAction
{
    [SerializeField] private bool _enabled;

    public override TutorialActionStatus Execute() {
        PlayerController.Instance.SetPlayerControllerEnabled(_enabled);
        return base.Execute();
    }
}
