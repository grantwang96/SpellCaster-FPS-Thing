using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorials/Actions/TogglePlayerHUD")]
public class TutorialActionTogglePlayerHUD : TutorialAction
{
    [SerializeField] private bool _enabled;

    public override TutorialActionStatus Execute() {
        PlayerHud.Instance.SetEnabled(_enabled);
        return base.Execute();
    }
}
