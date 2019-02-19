using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is wrapped by an overarching UIPanel object
/// </summary>
public class UISubPanel : MonoBehaviour {

    [SerializeField] protected UIPanel _parentPanel;
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

    }
}
