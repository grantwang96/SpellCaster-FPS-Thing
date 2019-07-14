using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Points at an interactable object
/// </summary>
public class InteractableMarker : MonoBehaviour {

    private IInteractable _target;
    private bool _onInteractSuccess;
    private Action _onInteractAction;

    public void InitializeMarker(IInteractable interactable, bool onInteractSuccess, Action onInteractAction) {
        _target = interactable;
        transform.position = interactable.InteractableCenter;
        _onInteractSuccess = onInteractSuccess;
        _onInteractAction = onInteractAction;
        if (_onInteractSuccess) {
            _target.OnInteractSuccess += OnTargetInteracted;
        } else {
            _target.OnInteractAttempt += OnTargetInteracted;
        }
    }

    private void FixedUpdate() {
        transform.position = _target.InteractableCenter;
    }

    private void OnTargetInteracted() {
        if (_onInteractSuccess) {
            _target.OnInteractSuccess -= OnTargetInteracted;
        } else {
            _target.OnInteractAttempt -= OnTargetInteracted;
        }
        _onInteractAction.Invoke();
        Destroy(this.gameObject);
    }
}
