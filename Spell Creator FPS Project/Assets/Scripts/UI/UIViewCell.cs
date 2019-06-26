using UnityEngine;

public abstract class UIViewCell : MonoBehaviour, IUIInteractable {

    [SerializeField] protected Animator _animator;

    protected string _id;
    public string Id => _id;
    [SerializeField] protected int xCoord;
    [SerializeField] protected int yCoord;
    public int XCoord => xCoord;
    public int YCoord => yCoord;

    public abstract void Initialize(int x, int y);
    public abstract void SetValue(IUIInteractableData data);
    public abstract void Highlight();
    public abstract void Unhighlight();
    public abstract IUIInteractableData ExtractData();
    public abstract void InteractableSelect();

    protected void PointerClick() {
        OnSelected?.Invoke(this);
    }

    protected void PointerEnter() {
        OnHighlighted?.Invoke(this);
    }

    protected void OnSelect() {
        OnSelected?.Invoke(this);
    }

    public event UIInteractableEvent OnSelected;
    public event UIInteractableEvent OnHighlighted;
}

public class ViewCellData : IUIInteractableData{
    public string Name;
    public string Id;

    public int X { get; set; }
    public int Y { get; set; }

    public ViewCellData(int x, int y) {
        X = x;
        Y = y;
    }
}
