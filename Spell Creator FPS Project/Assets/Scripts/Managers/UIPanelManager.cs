using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class UIPanelManager : MonoBehaviour {

    public const string GenericMessageBoxPrefabId = "prefab.GenericMessageBox";
    private const string UIPrefabsPath = "UI";

    public static UIPanelManager Instance { get; private set; }

    [SerializeField] private List<UIPanel> _uiPanelPrefabs = new List<UIPanel>();
    private Dictionary<string, UIPanel> _allUIPanels = new Dictionary<string, UIPanel>();
    private List<UIPanel> _activeUIPanels = new List<UIPanel>();
    public UIPanel CurrentPanel {
        get {
            return _activeUIPanels.Count > 0 ? _activeUIPanels[_activeUIPanels.Count - 1] : null;
        }
    }
    [SerializeField] private UIPanel _currentScenePanel;

    [SerializeField] private List<GameObject> _notificationPrefabGOs = new List<GameObject>();
    // dictionary that stores all notification prefabs
    private Dictionary<string, IUINotificationParent> _notificationParents = new Dictionary<string, IUINotificationParent>();

    [SerializeField] private Canvas _mainCanvas;
    [SerializeField] private Transform _interactablePanelsLayer;

    public delegate void PanelUpdateEvent(bool empty);
    public event Action<string> OnPanelDeactivated;
    public event Action<string> OnPanelActivated;
    public event PanelUpdateEvent OnPanelsUpdated;

    // events for passing data between UIPanels
    public event Action<string> OnStringDataPassed;

    private void Awake() {
        if(Instance != null) {
            Debug.LogWarning("WTF? More than one UIPanelManager exists in the scene!");
            return;
        }
        Instance = this;
    }

    private void Start() {
        // preload all required UI panels here
        PreloadUIPanels();
    }

    #region UI PANELS MANAGEMENT

    private void PreloadUIPanels() {
        for (int i = 0; i < _uiPanelPrefabs.Count; i++) {
            if (_allUIPanels.ContainsKey(_uiPanelPrefabs[i].name)) {
                Debug.LogError($"[UIManager] Contains duplicate prefab {_uiPanelPrefabs[i].name}");
                continue;
            }
            LoadUIPanelPrefab(_uiPanelPrefabs[i].name);
        }
    }

    public void OpenUIPanel(string prefabName, UIPanelInitData initData = null) {
        if(!_allUIPanels.TryGetValue(prefabName, out _currentScenePanel)) {
            if (!LoadUIPanelPrefab(prefabName)) {
                return;
            }
            _currentScenePanel = _allUIPanels[prefabName];
        }
        Debug.Log($"[{nameof(UIPanelManager)}] Opening ui panel {_currentScenePanel}");
        _currentScenePanel.Initialize(initData);
        _currentScenePanel.transform.SetAsLastSibling();
        _activeUIPanels.Add(_currentScenePanel);
        ActivateCurrentPanel();
        OnPanelsUpdated?.Invoke(_activeUIPanels.Count != 0);
    }

    private bool LoadUIPanelPrefab(string panelPrefabName) {
        string path = $"{UIPrefabsPath}/{panelPrefabName}";
        UIPanel panel = Resources.Load<UIPanel>(path);
        if(panel == null) {
            Debug.LogError($"[{nameof(UIPanelManager)}] Could not find panel in path {path}");
            return false;
        }
        UIPanel clone = Instantiate(panel, _interactablePanelsLayer);
        clone.gameObject.SetActive(false);
        clone.name = panelPrefabName;
        _allUIPanels.Add(panelPrefabName, clone);
        return true;
    }

    public void RegisterUIPanel(string prefabName) {
        if (_allUIPanels.ContainsKey(prefabName)) {
            return;
        }
        LoadUIPanelPrefab(prefabName);
    }

    // good for panels that are only used in one particular scene
    public void DeregisterUIPanel(string prefabName) {
        UIPanel panel;
        if(_allUIPanels.TryGetValue(prefabName, out panel)) {
            CloseUIPanel(prefabName);

        }
    }

    public void CloseUIPanel() {
        if (_activeUIPanels.Count != 0) {
            int indexLast = _activeUIPanels.Count - 1;
            UIPanel closingPanel = _activeUIPanels[indexLast];
            _activeUIPanels.RemoveAt(indexLast);
            DeactivateCurrentPanel();   
        }
        _currentScenePanel = null;
        if(_activeUIPanels.Count != 0) {
            _currentScenePanel = _activeUIPanels[_activeUIPanels.Count - 1];
            ActivateCurrentPanel();
        }
        OnPanelsUpdated?.Invoke(_activeUIPanels.Count != 0);
    }

    public void CloseUIPanel(string prefabId) {
        UIPanel panel;
        if(!_allUIPanels.TryGetValue(prefabId, out panel)) {
            Debug.LogError($"[{nameof(UIPanelManager)}] Received invalid prefab id {prefabId}");
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
            _currentScenePanel.SetPanelVisible(true);
        }
        // call some initialization function here
    }

    private void DeactivateCurrentPanel() {
        if (_currentScenePanel != null) {
            _currentScenePanel.SetPanelVisible(false);
            OnPanelDeactivated?.Invoke(_currentScenePanel.name);
        }
    }

    private void OnCancelPressed() {

    }

    #endregion
    
    public Vector2 GetCanvasPosition(Vector2 childPosition) {
        return _mainCanvas.transform.InverseTransformPoint(childPosition);
    }
}
