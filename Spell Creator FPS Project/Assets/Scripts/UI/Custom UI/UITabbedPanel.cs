using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITabbedPanel : UIPanel {

    [SerializeField] protected UIPanelTab[] _uiTabs;
    [SerializeField] private int _activeTabIndex;
    public int ActiveTabIndex { get { return _activeTabIndex; } }
    private float _holdTime;
    private const float _holdThreshold = 0.5f;

    public delegate void TabUpdatedEvent(int index);
    public event TabUpdatedEvent OnTabUpdated;

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
    }

    private void SideInputs() {
        // Controller only?
    }

    public void OnTabSelect(int index) {
        _activeTabIndex = index;
        OnTabUpdated?.Invoke(index);
    }
}
