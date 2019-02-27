using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is wrapped by an overarching UIPanel object
/// </summary>
public class UISubPanel : MonoBehaviour {

    [SerializeField] protected UISubPanelParent _parentPanel;
    public bool ActiveSubPanel;

    public delegate void MouseHover();
    public event MouseHover OnMouseHover;

    protected virtual void Update() {
        if (!ActiveSubPanel) {
            return;
        }
        ProcessInputs();
    }

    protected virtual void ProcessInputs() {

    }

    public virtual void Initialize(UIPanelInitData initData) {
        _parentPanel.OnSubPanelChanged += OnSubPanelChanged;
    }

    protected virtual void OnDestroy() {
        _parentPanel.OnSubPanelChanged -= OnSubPanelChanged;
    }

    protected virtual void OnSubPanelChanged() {

    }
}

public interface UIDirectionInput {

    float Horizontal { get; }
    float Vertical { get; }
    float IntervalHoldTime { get; }
}
