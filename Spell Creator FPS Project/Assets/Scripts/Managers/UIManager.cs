using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public const string GenericMessageBoxPrefabId = "prefab.GenericMessageBox";
    private const string UIPrefabsPath = "UI";

    public static UIManager Instance { get; private set; }

    [SerializeField] private List<UIPanel> _uiPanelPrefabs = new List<UIPanel>();
    private Dictionary<string, UIPanel> _allUIPanels = new Dictionary<string, UIPanel>();
    private List<UIPanel> _activeUIPanels = new List<UIPanel>();
    public UIPanel CurrentPanel {
        get {
            return _activeUIPanels.Count > 0 ? _activeUIPanels[_activeUIPanels.Count - 1] : null;
        }
    }
    [SerializeField] private UIPanel _currentScenePanel;
    [SerializeField] private string mainMenuPrefabName;
    [SerializeField] private Canvas _mainCanvas;

    public delegate void PanelUpdateEvent(bool empty);
    public event PanelUpdateEvent OnPanelsUpdated;

    // events for passing data between UIPanels
    public event Action<string> OnStringDataPassed;

    private void Awake() {
        if(Instance != null) {
            Debug.LogWarning("WTF? More than one UIManager exists in the scene!");
            return;
        }
        Instance = this;
    }

    private void Start() {
        // preload all required UI panels here
        for(int i = 0; i < _uiPanelPrefabs.Count; i++) {
            if (_allUIPanels.ContainsKey(_uiPanelPrefabs[i].name)) {
                Debug.LogError($"[UIManager] Contains duplicate prefab {_uiPanelPrefabs[i].name}");
                continue;
            }
            UIPanel panel = Instantiate(_uiPanelPrefabs[i], transform);
            panel.gameObject.SetActive(false);
            _allUIPanels.Add(_uiPanelPrefabs[i].name, panel);
        }
    }
    
    public void OpenUIPanel(string prefabName, UIPanelInitData initData = null) {
        if(!_allUIPanels.TryGetValue(prefabName, out _currentScenePanel)) {
            if (!LoadUIPrefab(prefabName)) {
                return;
            }
            _currentScenePanel = _allUIPanels[prefabName];
        }
        Debug.Log($"[{nameof(UIManager)}] Opening ui panel {_currentScenePanel}");
        _currentScenePanel.Initialize(initData);
        _currentScenePanel.transform.SetAsLastSibling();
        _activeUIPanels.Add(_currentScenePanel);
        ActivateCurrentPanel();
        OnPanelsUpdated?.Invoke(_activeUIPanels.Count != 0);
    }

    private bool LoadUIPrefab(string prefabName) {
        string path = $"{UIPrefabsPath}/{prefabName}";
        UIPanel panel = Resources.Load<UIPanel>(path);
        if(panel == null) {
            Debug.LogError($"[{nameof(UIManager)}] Could not find panel in path {path}");
            return false;
        }
        UIPanel clone = Instantiate(panel, transform);
        clone.gameObject.SetActive(false);
        _allUIPanels.Add(prefabName, clone);
        return true;
    }

    public void RegisterUIPanel(string prefabName) {
        if (_allUIPanels.ContainsKey(prefabName)) {
            return;
        }
        LoadUIPrefab(prefabName);
    }

    // good for panels that are only used in one particular scene
    public void DeregisterUIPanel(string prefabName) {
        UIPanel panel;
        if(_allUIPanels.TryGetValue(prefabName, out panel)) {
            CloseUIPanel(prefabName);

        }
    }

    public void CloseUIPanel() {
        Debug.Log($"Closing UIPanel...");
        if (_activeUIPanels.Count != 0) {
            int indexLast = _activeUIPanels.Count - 1;
            UIPanel closingPanel = _activeUIPanels[indexLast];
            _activeUIPanels.RemoveAt(indexLast);
            DeactivateCurrentPanel();   
        }
        if(_activeUIPanels.Count != 0) {
            _currentScenePanel = _activeUIPanels[_activeUIPanels.Count - 1];
            ActivateCurrentPanel();
        }
        OnPanelsUpdated?.Invoke(_activeUIPanels.Count != 0);
    }

    public void CloseUIPanel(string prefabId) {
        UIPanel panel;
        if(!_allUIPanels.TryGetValue(prefabId, out panel)) {
            Debug.LogError($"[{nameof(UIManager)}] Received invalid prefab id {prefabId}");
            return;
        }
        if (_activeUIPanels.Contains(panel)) {
            _activeUIPanels.Remove(panel);
        }
        panel.ClosePanel();
    }

    public void CloseAllUIPanels() {
        while(_activeUIPanels.Count != 0) {
            UIPanel panel = _activeUIPanels[_activeUIPanels.Count - 1];
            _activeUIPanels.RemoveAt(_activeUIPanels.Count - 1);
            panel.ClosePanel();
        }
        OnPanelsUpdated?.Invoke(_activeUIPanels.Count != 0);
    }

    public void PassStringData(string newString) {
        OnStringDataPassed?.Invoke(newString);
    }

    private void ActivateCurrentPanel() {
        if (_currentScenePanel != null) {
            _currentScenePanel.gameObject.SetActive(true);
        }
        // call some initialization function here
    }

    private void DeactivateCurrentPanel() {
        if (_currentScenePanel != null) {
            _currentScenePanel.gameObject.SetActive(false);
        }
    }

    private void OnCancelPressed() {

    }

    public Vector2 GetCanvasPosition(Vector2 childPosition) {
        return _mainCanvas.transform.InverseTransformPoint(childPosition);
    }
}
