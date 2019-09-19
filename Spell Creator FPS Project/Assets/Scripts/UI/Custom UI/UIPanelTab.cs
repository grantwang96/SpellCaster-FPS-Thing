using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelTab : MonoBehaviour {

    [SerializeField] private int _tabIndex;
    [SerializeField] private bool _isActiveTabPanel;

    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private UITabbedPanel _parentPanel; // the UITabbedPanel it is using
    [SerializeField] private UIPanel _childPanel; // the child that is created when this tab is selected
    [SerializeField] private Button _tabButton; // the button that allows selecting this tab
    [SerializeField] private RectTransform _content; // where the child panel will reside when instantiated
    [SerializeField] private UIPanel _childPanelPrefab;

    private void Awake() {

    }

    // Use this for initialization
    void Start () {
        _parentPanel.OnTabUpdated += OnTabUpdated;
        _tabButton.onClick.AddListener(OnTabSelected);

        if(_tabIndex == _parentPanel.ActiveTabIndex) {
            if(_childPanel == null) {
                _childPanel = Instantiate(_childPanelPrefab, _content);
            }
        }
    }
    
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTabSelected() {
        _parentPanel.OnTabSelect(_tabIndex);
    }

    private void OnTabUpdated(int index) {
        _isActiveTabPanel = index == _tabIndex;
        if(_isActiveTabPanel) {
            ActivateContent();
        } else {
            DeactivateContent();
        }
    }

    private void ActivateContent() {
        if (_childPanel == null) {
            _childPanel = Instantiate(_childPanelPrefab, _content);
        }
        transform.SetAsLastSibling();
        Debug.Log($"{name} has been selected!");
    }

    private void DeactivateContent() {
        if (_childPanel != null) {
            Destroy(_childPanel.gameObject);
            _childPanel = null;
        }
    }
}
