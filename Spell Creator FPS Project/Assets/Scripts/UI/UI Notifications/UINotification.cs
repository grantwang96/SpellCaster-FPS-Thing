using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINotification : MonoBehaviour
{
    protected const string DisplayAnimationState = "Display";
    protected const string UpdateAnimationState = "Update";
    protected const string HideAnimationState = "Hide";

    [SerializeField] private Text _text;
    [SerializeField] private Image _icon;
    [SerializeField] private Animator _animator;
    [SerializeField] private bool _hideOnDisplayEnd;

    private IUINotificationParent _parent;
    private string _uniqueId;

    public virtual void Initialize(IUINotificationParent parent) {
        _parent = parent;
        gameObject.SetActive(false);
    }

    public void Display(string uniqueId, NotificationInitData initData) {
        _uniqueId = uniqueId;
        _text.text = initData.Message;
        _icon.sprite = initData.Icon;
        // play display animation
        transform.SetAsLastSibling();
        gameObject.SetActive(true);
        _animator.Play(DisplayAnimationState);
    }

    public virtual void OnDisplayAnimationFinished() {
        // do something here when animation finishes
        if (_hideOnDisplayEnd) {
            _parent.RemoveNotification(_uniqueId);
        }
    }

    public void UpdateData(NotificationInitData initData) {
        _text.text = initData.Message;
        _icon.sprite = initData.Icon;
        // play update animation
        _animator.Play(UpdateAnimationState);
    }

    public void Hide() {
        // play remove animation
        _animator.Play(HideAnimationState);
    }

    public virtual void OnHideAnimationFinished() {
        gameObject.SetActive(false);
    }
}
