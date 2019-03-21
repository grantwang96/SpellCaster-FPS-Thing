using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICustomButton : Button, IUIInteractable, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] private string _id;
    public string Id => _id;

    [SerializeField] private RectTransform _rect;

    public event UIInteractableEvent OnSelected;
    public event UIInteractableEvent OnHighlighted;

    public void Initialize(int x, int y) {
        _rect = GetComponent<RectTransform>();
    }

    public void SetValue(IUIInteractableData initData) {

    }

    public void Highlight() {
        _rect.localScale = Vector3.one * 1.25f;
    }

    public void Unhighlight() {
        _rect.localScale = Vector3.one;
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

    // Use this for initialization


    // Update is called once per frame
    void Update () {
		
	}

    public IUIInteractableData ExtractData() {
        throw new System.NotImplementedException();
    }
}
