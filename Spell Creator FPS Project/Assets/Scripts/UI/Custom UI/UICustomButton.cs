using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Button that is designed to work with a view grid
/// </summary>
public class UICustomButton : Button, IUIInteractable {

    public int XCoord { get; private set; }
    public int YCoord { get; private set; }

    [SerializeField] private string _id;
    public string Id => _id;

    public Vector2 Position => UIManager.Instance.GetCanvasPosition(_rect.position);

    [SerializeField] private RectTransform _rect;
    public RectTransform RectTransform => _rect;
    [SerializeField] private Text _customButtonText;

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
        UICustomButtonInitData customButtonData = initData as UICustomButtonInitData;
        if(customButtonData == null) {
            return;
        }
        XCoord = customButtonData.X;
        YCoord = customButtonData.Y;
        _id = customButtonData.Id;
        _customButtonText.text = customButtonData.ButtonText;
    }

    // when dpad/arrow inputs are used
    public void InteractableHighlight() {
        Highlight();
    }

    // the actual highlight action. Do not call event here.
    private void Highlight() {
        _rect.localScale = Vector3.one * 1.25f;
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void InteractableUnhighlight() {
        UnHighlight();
    }

    private void UnHighlight() {
        _rect.localScale = Vector3.one;
    }

    // called via button input (not mouse)
    public void InteractableSelect() {
        // onClick?.Invoke();
        OnButtonSelect();
    }

    public override void OnPointerClick(PointerEventData eventData) {
        base.OnPointerClick(eventData);
        OnButtonSelect();
        eventData.Use();
    }

    private void OnButtonSelect() {
        OnMousePointerClick?.Invoke(this);
    }

    public override void OnPointerEnter(PointerEventData eventData) {
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

public class ButtonActionData {
    public string ButtonId;
    public string ButtonText;
    public UnityAction Action;
}