using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorials/Actions/SetCamera")]
public class TutorialActionSetCamera : TutorialAction
{
    [SerializeField] private string _cameraId;

    public override TutorialActionStatus Execute() {
        if (!CameraManager.Instance.SceneCameras.ContainsKey(_cameraId)) {
            return TutorialActionStatus.Abort;
        }
        CameraManager.Instance.SetActiveCamera(_cameraId);
        Debug.Log($"Setting active camera {_cameraId}");
        return TutorialActionStatus.Complete;
    }
}
