using UnityEngine;

public abstract class UIViewCell : MonoBehaviour, IUIInteractable {

    [SerializeField] protected Animator _animator;
    [SerializeField] protected RectTransform _rect;
    public RectTransform RectTransform => _rect;

    protected string _id;
    public string Id => _id;
    [SerializeField] protected int xCoord;
    [SerializeField] protected int yCoord;
    public int XCoord => xCoord;
    public int YCoord => yCoord;

    public Vector2 Position => UIManager.Instance.GetCanvasPosition(_rect.position);

    public abstract void Initialize(int x, int y);
    public abstract void SetValue(IUIInteractableData data);
    public abstract void InteractableHighlight();
    public abstract void InteractableUnhighlight();
    public abstract IUIInteractableData ExtractData();
    public abstract void InteractableSelect();
    
    protected void PointerClick() {
        OnMousePointerClick?.Invoke(this);
    }

    protected void PointerEnter() {
        OnMousePointerHighlight?.Invoke(this);
    }

    protected void OnSelect() {
        OnMousePointerClick?.Invoke(this);
    }

    public event UIInteractableEvent OnMousePointerClick;
    public event UIInteractableEvent OnMousePointerHighlight;
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
