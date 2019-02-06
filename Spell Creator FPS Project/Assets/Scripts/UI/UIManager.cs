using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public static UIManager Instance;

    private List<UIPanel> UIPanels = new List<UIPanel>();
    private Stack<UIPanel> _uiPanels = new Stack<UIPanel>();
    public UIPanel CurrentPanel => _uiPanels.Peek();
    [SerializeField] private UIPanel _currentScenePanel;

    public delegate void PanelUpdateEvent(bool empty);
    public event PanelUpdateEvent OnPanelsUpdated;

    void Awake() {
        if(Instance != null) {
            Debug.LogWarning("More than one UIManager exists in the scene!");
            return;
        }
        Instance = this;
    }

    void Update() {

    }

    public void OpenUIPanel(UIPanel uiPanelPrefab) {
        _currentScenePanel = Instantiate(uiPanelPrefab, transform);
        _uiPanels.Push(_currentScenePanel);
        ActivateCurrentPanel();
        OnPanelsUpdated?.Invoke(_uiPanels.Count == 0);
    }

    public void CloseUIPanel() {
        if(_uiPanels.Count != 0) {
            UIPanel closingPanel = _uiPanels.Pop();
            Destroy(closingPanel.gameObject);
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
