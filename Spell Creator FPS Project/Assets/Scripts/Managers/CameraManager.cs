using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICameraManager {
    IReadOnlyDictionary<string, Camera> SceneCameras { get; }
    Camera ActiveCamera { get; }

    void SetActiveCamera(string cameraId);
    void SetCameraActive(string cameraId, bool active);
    Camera GetCameraById(string cameraId);
}

public class CameraManager : MonoBehaviour, ICameraManager
{
    public const string MainCameraId = "Main Camera";
    public static ICameraManager Instance { get; private set; }

    public Camera ActiveCamera { get; private set; }
    [SerializeField] private List<Camera> _allCameras = new List<Camera>();
    private Dictionary<string, Camera> _sceneCameras = new Dictionary<string, Camera>();
    public IReadOnlyDictionary<string, Camera> SceneCameras => _sceneCameras;

    private void Awake()
    {
        Instance = this;
        InitializeCameras();
    }

    private void InitializeCameras() {
        for(int i = 0; i < _allCameras.Count; i++) {
            _sceneCameras.Add(_allCameras[i].name, _allCameras[i]);
        }
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

    public void SetCameraActive(string cameraId, bool active) {
        if(SceneCameras.TryGetValue(cameraId, out Camera camera)) {
            camera.enabled = active;
        }
    }

    public Camera GetCameraById(string cameraId) {
        if(SceneCameras.TryGetValue(cameraId, out Camera camera)) {
            return camera;
        }
        return null;
    }
}
