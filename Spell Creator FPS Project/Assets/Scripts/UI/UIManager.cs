﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public static UIManager Instance;

    [SerializeField] private List<UIPanel> _uiPanelPrefabs = new List<UIPanel>();
    private Dictionary<string, UIPanel> _allUIPanels = new Dictionary<string, UIPanel>();
    private Stack<UIPanel> _uiPanels = new Stack<UIPanel>();
    public UIPanel CurrentPanel => _uiPanels.Peek();
    [SerializeField] private UIPanel _currentScenePanel;

    public delegate void PanelUpdateEvent(bool empty);
    public event PanelUpdateEvent OnPanelsUpdated;

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

    void Update() {

    }

    public void OpenUIPanel(string prefabName, UIPanelInitData initData = null) {
        // _currentScenePanel = Instantiate(uiPanelPrefab, transform);
        if(!_allUIPanels.TryGetValue(prefabName, out _currentScenePanel)) {
            Debug.LogError($"[UIManager] Could not retrireve panel for name {prefabName}");
            return;
        }
        _currentScenePanel.Initialize(initData);
        _currentScenePanel.transform.SetAsLastSibling();
        _uiPanels.Push(_currentScenePanel);
        ActivateCurrentPanel();
        OnPanelsUpdated?.Invoke(_uiPanels.Count == 0);
    }

    public void CloseUIPanel() {
        if(_uiPanels.Count != 0) {
            UIPanel closingPanel = _uiPanels.Pop();
            // Destroy(closingPanel.gameObject);
            DeactivateCurrentPanel();
        }
        if(_uiPanels.Count != 0) {
            _currentScenePanel = _uiPanels.Peek();
            ActivateCurrentPanel();
        }
        OnPanelsUpdated?.Invoke(_uiPanels.Count == 0);
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
}
