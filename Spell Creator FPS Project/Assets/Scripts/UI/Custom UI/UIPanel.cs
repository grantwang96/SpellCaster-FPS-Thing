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

    private bool _active;

	// Use this for initialization
	protected virtual void Start () {
        _closeButton?.onClick.AddListener(ClosePanel);
    }

    public virtual void Initialize(UIPanelInitData initData) {
        UIManager.Instance.OnPanelsUpdated += OnUIManagerPanelsUpdated;
        SubscribeToGameplayController();
        OnControllerStateUpdated();
    }

    protected virtual void SubscribeToGameplayController() {
        PlayerController.Instance.OnControllerStateUpdated += OnControllerStateUpdated;
        PlayerController.Instance.OnCancelPressed += OnMenuBtnPressed;
    }

    protected virtual void UnsubscribeToGameplayController() {
        PlayerController.Instance.OnCancelPressed -= OnMenuBtnPressed;
        PlayerController.Instance.OnControllerStateUpdated -= OnControllerStateUpdated;
    }

    protected virtual void OnUIManagerPanelsUpdated(bool panels) {
        UnsubscribeToGameplayController();
        if (panels && UIManager.Instance.CurrentPanel == this) {
            SubscribeToGameplayController();
        }
    }

    public virtual void ClosePanel() {
        UIManager.Instance.CloseUIPanel();
        UIManager.Instance.OnPanelsUpdated -= OnUIManagerPanelsUpdated;
        UnsubscribeToGameplayController();
    }

    public virtual void SetPanelVisible(bool visible) {
        gameObject.SetActive(visible);
    }

    protected virtual void MenuBtnAction() {
        ClosePanel();
    }

    protected virtual void OnMenuBtnPressed() {
        if (!_active) { return; }
        MenuBtnAction();
    }

    protected virtual void OnControllerStateUpdated() {
        _active = PlayerController.Instance.ControllerState == ControllerState.UIMenu;
    }
}

public abstract class UISubPanelParent : UIPanel {

    public event Action OnPanelClosed;
    public event Action<bool> OnCurrentPanelUpdated;
    
    public virtual void ChangePanel(UISubPanel subPanel, IntVector3 dir, bool hardLocked = false) {

    }

    protected override void OnUIManagerPanelsUpdated(bool panels) {
        base.OnUIManagerPanelsUpdated(panels);
        bool isCurrentPanel = UIManager.Instance.CurrentPanel == this;
        OnCurrentPanelUpdated?.Invoke(isCurrentPanel);
        if (isCurrentPanel) {
            // do active panel things
        }
    }

    public override void ClosePanel() {
        base.ClosePanel();
        OnPanelClosed?.Invoke();
    }
}

// Helper class used to initialize panel data
public class UIPanelInitData {

}