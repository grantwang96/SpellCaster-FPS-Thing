using UnityEngine;

public abstract class UIViewCell : MonoBehaviour {

    [SerializeField] protected Animator _animator;

    public abstract void Initialize(ViewCellInitData initData);
    public abstract void Highlight();
    public abstract void Unhighlight();
}

public class ViewCellInitData {
    public string Name;
    public string itemId;
}
