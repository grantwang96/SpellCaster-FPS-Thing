using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Parent class for all panels that take over PLayer gameplay controls
/// </summary>
public abstract class UIPanel : MonoBehaviour {

    [SerializeField] protected Button _closeButton;

    public Vector2 _directionalInput { get; protected set; }

	// Use this for initialization
	protected virtual void Start () {
        _closeButton?.onClick.AddListener(CloseUIPanel);
    }

    public virtual void Initialize(UIPanelInitData initData) {

    }
	
	// Update is called once per frame
	protected virtual void Update () {
        ProcessInputs();
	}

    protected virtual void OnUIManagerPanelsUpdated(bool panels) {
        if(panels && UIManager.Instance.CurrentPanel == this) {
            Debug.Log($"Panel {this.name} is current active panel.");
        }
    }

    protected virtual void ProcessInputs() {
        // do not run if this is not current panel
        if (UIManager.Instance.CurrentPanel != this) {
            return;
        }
        ProcessMenuButton();
    }

    private void ProcessMenuButton() {
        if (Input.GetButtonDown("Cancel")) {
            CloseUIPanel();
        }
    }

    protected virtual void CloseUIPanel() {
        UIManager.Instance.CloseUIPanel();
    }
}

public abstract class UISubPanelParent : UIPanel {

    public event Action OnPanelClosed;
    
    public virtual void ChangePanel(UISubPanel subPanel, IntVector3 dir, bool hardLocked = false) {

    }

    protected override void CloseUIPanel() {
        base.CloseUIPanel();
        OnPanelClosed?.Invoke();
    }
}

// Helper class used to initialize panel data
public class UIPanelInitData {

}