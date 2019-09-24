using UnityEngine;

public delegate void UIInteractableEvent(IUIInteractable interactable);

public interface IUIInteractable {

    int XCoord { get; }
    int YCoord { get; }
    string Id { get; }
    Vector2 Position { get; }
    RectTransform RectTransform { get; }
    GameObject gameObject { get; }

    void Initialize(int x, int y);
    void SetValue(IUIInteractableData data);
    void InteractableHighlight();
    void InteractableUnhighlight();
    void InteractableSelect();
    IUIInteractableData ExtractData();

    event UIInteractableEvent OnMousePointerClick;
    event UIInteractableEvent OnMousePointerHighlight;
}

public interface IUIInteractableData {
    int X { get; set; }
    int Y { get; set; }
}
