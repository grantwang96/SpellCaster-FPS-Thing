using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Button that is designed to work with a view grid
/// </summary>
public class UICustomButton : Button, IUIInteractable {

    public int XCoord { get; private set; }
    public int YCoord { get; private set; }

    [SerializeField] private string _id;
    public string Id => _id;

    [SerializeField] private RectTransform _rect;

    // events related to data represented by UIInteractable
    public event UIInteractableEvent OnMousePointerClick;
    public event UIInteractableEvent OnMousePointerHighlight;

    protected override void Awake() {
        base.Awake();
        _rect = GetComponent<RectTransform>();
    }

    public void Initialize(int x, int y) {
        XCoord = x;
        YCoord = y;
        _rect = GetComponent<RectTransform>();
    }

    public void SetValue(IUIInteractableData initData) {

    }

    // when dpad/arrow inputs are used
    public void InteractableHighlight() {
        Highlight();
    }

    // the actual highlight action. Do not call event here.
    private void Highlight() {
        _rect.localScale = Vector3.one * 1.25f;
    }

    public void InteractableUnhighlight() {
        UnHighlight();
    }

    private void UnHighlight() {
        _rect.localScale = Vector3.one;
    }

    public void InteractableSelect() {
        onClick?.Invoke();
        OnButtonSelect();
    }

    public override void OnPointerClick(PointerEventData eventData) {
        Debug.Log($"Clicked on {this.name}");
        base.OnPointerClick(eventData);
        OnButtonSelect();
    }

    private void OnButtonSelect() {
        OnMousePointerClick?.Invoke(this);
    }

    public override void OnPointerEnter(PointerEventData eventData) {
        Debug.Log($"Hovering over {this.name}");
        base.OnPointerEnter(eventData);
        OnMousePointerHighlight?.Invoke(this);
    }

    public IUIInteractableData ExtractData() {
        throw new System.NotImplementedException();
    }

    public void ClearHighlightedEvents() {
        OnMousePointerHighlight = null;
    }

    public void ClearSelectedEvents() {
        OnMousePointerClick = null;
    }
}

public class UICustomButtonInitData : IUIInteractableData {

    public int X { get; set; }
    public int Y { get; set; }
    public string Id { get; set; }
    public string ButtonText { get; set; }
}