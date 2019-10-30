using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorials/Actions/WaitForSeconds")]
public class TutorialActionWait : TutorialAction
{
    [SerializeField] private float _seconds;

    public override TutorialActionStatus Execute() {
        CoroutineGod.Instance.ExecuteAfterTime(TutorialActionCompleted, _seconds);
        return TutorialActionStatus.Incomplete;
    }
}
