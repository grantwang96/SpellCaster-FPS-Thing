using UnityEngine;

public delegate void UIInteractableEvent(IUIInteractable interactable);

public interface IUIInteractable {

    string Id { get; }
    GameObject gameObject { get; }

    void Initialize(int x, int y);
    void SetValue(IUIInteractableData data);
    void Highlight();
    void Unhighlight();
    IUIInteractableData ExtractData();

    event UIInteractableEvent OnSelected;
    event UIInteractableEvent OnHighlighted;
}

public interface IUIInteractableData {
    int X { get; set; }
    int Y { get; set; }
}
