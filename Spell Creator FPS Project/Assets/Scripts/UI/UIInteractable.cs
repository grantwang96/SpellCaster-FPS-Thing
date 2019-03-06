public interface UIInteractable {

    string Id { get; }

    void Initialize(UIInteractableInitData initData);
    void Highlight();
    void Unhighlight();
}

public class UIInteractableInitData {

}
