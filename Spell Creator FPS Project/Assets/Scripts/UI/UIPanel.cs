using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Parent class for all panels that take over PLayer gameplay controls
/// </summary>
public abstract class UIPanel : MonoBehaviour {

    [SerializeField] protected Button _closeButton;

	// Use this for initialization
	protected virtual void Start () {
        Debug.Log("Add listener");
        _closeButton?.onClick.AddListener(CloseUIPanel);
    }

    public virtual void Initialize(UIPanelInitData initData) {

    }
	
	// Update is called once per frame
	protected virtual void Update () {
        ProcessMenuButton();
	}

    protected virtual void OnUIManagerPanelsUpdated(bool panels) {
        if(panels && UIManager.Instance.CurrentPanel == this) {
            Debug.Log($"Panel {this.name} is current active panel.");
        }
    }

    private void ProcessMenuButton() {
        if (Input.GetButtonDown("Cancel")) {
            CloseUIPanel();
        }
    }

    protected virtual void CloseUIPanel() {
        UIManager.Instance.CloseUIPanel();
        _closeButton?.onClick.RemoveListener(CloseUIPanel);
    }
}

public abstract class UISubPanelParent : UIPanel {
    
    public virtual void ChangePanel(UISubPanel subPanel, IntVector3 dir) {

    }
}

// Helper class used to initialize panel data
public class UIPanelInitData {

}

public class InventoryPanelInitData : UIPanelInitData {
    public IRunicInventory Inventory;
    public InventoryItemType[] Filter;
}