using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(menuName = "Tutorials/Actions/Cinematic")]
public class TutorialActionCinematic : TutorialAction
{
    [SerializeField] private string _cinematicsId;
    [SerializeField] private bool _interruptable;
    [SerializeField] private float _interruptTime;

    public override TutorialActionStatus Execute() {
        PlayableDirector director;
        if (!CinematicsManager.Instance.Directors.TryGetValue(_cinematicsId, out director)) {
            ErrorManager.LogGameObjectError(name, $"Could not find Playable Director with id: {_cinematicsId}!");
            return TutorialActionStatus.Abort;
        }
        director.Play();
        if (_interruptable) {
            CoroutineGod.Instance.ExecuteAfterTime(TutorialActionCompleted, _interruptTime);
        } else {
            director.stopped += OnDirectorFinishedPlaying;
        }
        return TutorialActionStatus.Incomplete;
    }

    private void OnDirectorFinishedPlaying(PlayableDirector director) {
        TutorialActionCompleted();
        director.stopped -= OnDirectorFinishedPlaying;
    }

    public override void Abort() {
        base.Abort();
        PlayableDirector director;
        if (!CinematicsManager.Instance.Directors.TryGetValue(_cinematicsId, out director)) {
            ErrorManager.LogGameObjectError(name, $"Could not find Playable Director with id: {_cinematicsId}!");
            return;
        }
        director.Stop();
    }
}