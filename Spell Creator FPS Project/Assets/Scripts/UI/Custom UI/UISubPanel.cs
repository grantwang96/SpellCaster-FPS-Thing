using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This class is wrapped by an overarching UIPanel object
/// </summary>
public class UISubPanel : MonoBehaviour, IPointerEnterHandler {

    [SerializeField] protected UISubPanelParent _parentPanel;
    public bool IsFocused { get; protected set; }

    public delegate void MouseHover();
    public event MouseHover OnMouseHover;

    [SerializeField] protected UISubPanel _upNeighbor;
    [SerializeField] protected UISubPanel _rightNeighbor;
    [SerializeField] protected UISubPanel _downNeighbor;
    [SerializeField] protected UISubPanel _leftNeighbor;

    protected virtual void Update() {
        if (!IsFocused) {
            return;
        }
        ProcessInputs();
    }

    protected virtual void ProcessInputs() {
        
    }

    public virtual void Initialize(UIPanelInitData initData) {
        
    }

    public virtual void SetFocus(bool isFocused, bool hardLocked, IntVector3 dir) {
        IsFocused = isFocused;
    }

    public virtual void SetVisible(bool visible) {
        gameObject.SetActive(visible);
    }

    public virtual void OnPointerEnter(PointerEventData eventData) {
        
    }
}

public interface UIDirectionInput {

    float Horizontal { get; }
    float Vertical { get; }
    float IntervalHoldTime { get; }
}
