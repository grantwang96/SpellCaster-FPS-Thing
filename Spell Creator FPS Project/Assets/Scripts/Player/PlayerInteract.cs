using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour {

    [SerializeField] private float _interactDistance;
    [SerializeField] private LayerMask _interactLayers;

    private Transform _head;
    private IRaycastInteractable _currentInteractable;
    private bool _active;

    private void OnDisable() {
        UnsubscribeToController();
    }

    private void SubscribeToController() {
        PlayerController.Instance.OnInteractPressed += PressInteract;
        PlayerController.Instance.OnInteractHeld += HoldInteract;
        PlayerController.Instance.OnControllerStateUpdated += OnControllerStateUpdate;
        PlayerController.Instance.Damageable.OnDeath += OnPlayerDeath;
    }

    private void UnsubscribeToController() {
        PlayerController.Instance.OnInteractPressed -= PressInteract;
        PlayerController.Instance.OnInteractHeld -= HoldInteract;
        PlayerController.Instance.OnControllerStateUpdated -= OnControllerStateUpdate;
        PlayerController.Instance.Damageable.OnDeath -= OnPlayerDeath;
    }

    // Use this for initialization
    void Start () {
        _head = PlayerController.Instance.Head;
        SubscribeToController();
        OnControllerStateUpdate();
	}
	
	// Update is called once per frame
	void Update () {
        Look();
	}

    private void Look() {
        Ray ray = new Ray(_head.position, _head.forward);
        RaycastHit rayHit;
        if(Physics.Raycast(ray, out rayHit, _interactDistance, _interactLayers, QueryTriggerInteraction.Collide)) {
            IRaycastInteractable interactable = rayHit.collider.GetComponent<IRaycastInteractable>();
            if (interactable != null) {
                _currentInteractable = interactable;
                interactable.Detect(PlayerController.Instance);
            }
        } else {
            _currentInteractable?.Undetect();
            _currentInteractable = null;
        }
    }

    private void PressInteract() {
        if (!_active) { return; }
        if (_currentInteractable != null) {
            _currentInteractable.InteractPress(PlayerController.Instance);
        }
    }

    private void HoldInteract() {
        if (!_active) { return; }
        if (_currentInteractable != null) {
            _currentInteractable.InteractHold(PlayerController.Instance);
        }
    }

    private void ReleaseInteract() {
        if (!_active) { return; }

    }

    private void OnControllerStateUpdate() {
        _active = PlayerController.Instance.ControllerState == ControllerState.Gameplay;
    }

    private void OnPlayerDeath(bool isDead, Damageable damageable) {
        UnsubscribeToController();
    }
}
