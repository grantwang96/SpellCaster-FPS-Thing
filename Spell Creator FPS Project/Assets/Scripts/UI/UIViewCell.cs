using UnityEngine;

public abstract class UIViewCell : MonoBehaviour, UIInteractable {

    [SerializeField] protected Animator _animator;

    public string Id {
        get {
            throw new System.NotImplementedException();
        }
    }

    public abstract void Initialize(UIInteractableInitData initData);
    public abstract void Highlight();
    public abstract void Unhighlight();
}

public class ViewCellInitData : UIInteractableInitData{
    public string Name;
    public string itemId;
}
