using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorials/Actions/SetCamera")]
public class TutorialActionSetCamera : TutorialAction
{
    [SerializeField] private string _cameraId;
    [SerializeField] private bool _active;

    public override TutorialActionStatus Execute() {
        if (!CameraManager.Instance.SceneCameras.ContainsKey(_cameraId)) {
            return TutorialActionStatus.Abort;
        }
        // CameraManager.Instance.SetActiveCamera(_cameraId);
        CameraManager.Instance.SetCameraActive(_cameraId, _active);
        return TutorialActionStatus.Complete;
    }

    public override void Abort() {
        base.Abort();
        // CameraManager.Instance.SetActiveCamera(CameraManager.MainCameraId);
        CameraManager.Instance.SetCameraActive(_cameraId, false);
        CameraManager.Instance.SetCameraActive(CameraManager.MainCameraId, true);
    }
}
