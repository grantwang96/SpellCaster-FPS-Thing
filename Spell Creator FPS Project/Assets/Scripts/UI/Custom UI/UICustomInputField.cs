using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICustomInputField : InputField, IUIInteractable, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

    public int XCoord { get; private set; }
    public int YCoord { get; private set; }

    [SerializeField] private string _id; // the idea of an item here. This field will likely not matter much
    public string Id => _id;

    public event UIInteractableEvent OnMousePointerClick;
    public event UIInteractableEvent OnMousePointerHighlight;

    public IUIInteractableData ExtractData() {
        throw new System.NotImplementedException();
    }

    public void InteractableHighlight() {

    }

    public void Initialize(int x, int y) {

    }

    public void SetValue(IUIInteractableData data) {

    }

    public void InteractableUnhighlight() {

    }

    public void InteractableSelect() {

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
