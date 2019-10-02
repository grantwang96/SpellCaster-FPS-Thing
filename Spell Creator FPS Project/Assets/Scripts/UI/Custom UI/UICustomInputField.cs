using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICustomInputField : InputField, IUIInteractable, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

    public int XCoord { get; private set; }
    public int YCoord { get; private set; }

    public bool StartActive;

    [SerializeField] private string _id; // the idea of an item here. This field will likely not matter much
    public string Id => _id;
    [SerializeField] private RectTransform _rect;
    public RectTransform RectTransform => _rect;
    public Vector2 Position => UIManager.Instance.GetCanvasPosition(_rect.position);

    public event UIInteractableEvent OnMousePointerClick;
    public event UIInteractableEvent OnMousePointerHighlight;

    protected override void OnEnable() {
        base.OnEnable();
        if (StartActive) {
            InteractableSelect();
        }
    }

    public IUIInteractableData ExtractData() {
        return null;
    }

    public void InteractableHighlight() {
        EventSystem.current.SetSelectedGameObject(gameObject, null);
    }

    public void Initialize(int x, int y) {
        XCoord = x;
        YCoord = y;
    }

    public void SetValue(IUIInteractableData data) {

    }

    public void InteractableUnhighlight() {
        DeactivateInputField();
        OnDeselect(null);
    }

    public void InteractableSelect() {
        ActivateInputField();
        OnMousePointerClick?.Invoke(this);
    }

    public override void OnPointerClick(PointerEventData eventData) {
        Debug.Log($"Clicked on {this.name}");
        base.OnPointerClick(eventData);
        OnMousePointerClick?.Invoke(this);
    }

    public override void OnPointerEnter(PointerEventData eventData) {
        Debug.Log($"Hovering over {this.name}");
        base.OnPointerEnter(eventData);
        OnMousePointerHighlight?.Invoke(this);
    }
}

public class UICustomInputFieldData : IUIInteractableData {
    public int X { get; set; }
    public int Y { get; set; }
    public string Id { get; set; }
    public string InputFieldText { get; set; }
}
