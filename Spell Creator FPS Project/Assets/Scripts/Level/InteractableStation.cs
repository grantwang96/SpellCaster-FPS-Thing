using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableStation : MonoBehaviour, IInteractable {
    
    public event InteractEvent OnInteractAttempt;
    public event InteractEvent OnInteractSuccess;

    public bool Interactable { get; protected set; }

    [SerializeField] protected string _overrideId;
    public string InteractableId { get; protected set; }

    public abstract Vector3 InteractableCenter { get; }

    private void Awake() {
        
    }

    private void Start() {
        SetId();
    }

    protected virtual void SetId() {
        if (!_overrideId.Equals(string.Empty)) {
            InteractableId = _overrideId;
        } else {
            InteractableId =
                $"{GameplayValues.Level.LoadoutStationInstanceIdPrefix}{StringGenerator.RandomString(GameplayValues.Level.LoadoutStationInstanceIdSize)}";
        }
        LevelManager.Instance.RegisterInteractable(this);
    }

    public virtual void InteractPress(CharacterBehaviour character) {
        OnInteractAttempt?.Invoke();
        if (character != PlayerController.Instance) {
            return;
        }
        OnPlayerInteract();
        OnInteractSuccess?.Invoke();
    }

    public virtual void InteractHold(CharacterBehaviour character) {

    }

    protected abstract void OnPlayerInteract();

    public virtual void Detect() {

    }

    protected virtual void OnPanelClosed() {

    }
}
