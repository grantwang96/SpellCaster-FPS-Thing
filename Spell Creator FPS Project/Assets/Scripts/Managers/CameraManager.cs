using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICameraManager {
    IReadOnlyList<Camera> SceneCameras { get; }
    Camera ActiveCamera { get; }

    void SetActiveCamera(string cameraId);
    Camera GetCameraById(string cameraId);
}

public class CameraManager : MonoBehaviour, ICameraManager
{
    private const string MainCameraId = "MainCamera";
    public static ICameraManager Instance { get; private set; }

    public Camera ActiveCamera { get; private set; }
    [SerializeField] private List<Camera> _sceneCameras = new List<Camera>();
    public IReadOnlyList<Camera> SceneCameras => _sceneCameras;

    private void Awake()
    {
        Instance = this;
        SetActiveCamera(MainCameraId);
    }

    public void SetActiveCamera(string cameraId) {
        Camera camera = GetCameraById(cameraId);
        if(camera == null) {
            ErrorManager.LogError(nameof(CameraManager), $"Camera with id \"{cameraId}\" could not be found!");
            return;
        }
        if(ActiveCamera != null) {
            ActiveCamera.enabled = false;
        }
        camera.enabled = true;
        ActiveCamera = camera;
    }

    public Camera GetCameraById(string cameraId) {
        return _sceneCameras.Find(x => x.name == cameraId);
    }
}
