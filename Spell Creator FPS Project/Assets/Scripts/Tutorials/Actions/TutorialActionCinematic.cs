using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(menuName = "Tutorials/Actions/Cinematic")]
public class TutorialActionCinematic : TutorialAction
{
    [SerializeField] private string _cameraId;

    public override TutorialActionStatus Execute() {
        PlayableDirector director;
        return TutorialActionStatus.Incomplete;
    }
}