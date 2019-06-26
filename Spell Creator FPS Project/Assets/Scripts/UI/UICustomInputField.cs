using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICustomInputField : InputField, IUIInteractable, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] private string _id; // the idea of an item here. This field will likely not matter much
    public string Id => _id;

    public event UIInteractableEvent OnSelected;
    public event UIInteractableEvent OnHighlighted;

    public IUIInteractableData ExtractData() {
        throw new System.NotImplementedException();
    }

    public void Highlight() {

    }

    public void Initialize(int x, int y) {

    }

    public void SetValue(IUIInteractableData data) {

    }

    public void Unhighlight() {

    }

    public void InteractableSelect() {

    }

    public override void OnPointerClick(PointerEventData eventData) {
        Debug.Log($"Clicked on {this.name}");
        base.OnPointerClick(eventData);
        OnSelected?.Invoke(this);
    }

    public override void OnPointerEnter(PointerEventData eventData) {
        Debug.Log($"Hovering over {this.name}");
        base.OnPointerEnter(eventData);
        OnHighlighted?.Invoke(this);
    }
}
