using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour {

    [SerializeField] private float interactDistance;
    [SerializeField] private LayerMask interactLayers;

    private Transform head;

    private IInteractable currentInteractable;

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
        head = PlayerController.Instance.Head;
        SubscribeToController();
        OnControllerStateUpdate();
	}
	
	// Update is called once per frame
	void Update () {
        Look();
	}

    private void Look() {
        Ray ray = new Ray(head.position, head.forward);
        RaycastHit rayHit;
        if(Physics.Raycast(ray, out rayHit, interactDistance, interactLayers, QueryTriggerInteraction.Collide)) {
            IInteractable interactable = rayHit.collider.GetComponent<IInteractable>();
            if (interactable != null) {
                currentInteractable = interactable;
                interactable.Detect();
            }
        } else {
            currentInteractable = null;
        }
    }

    private void PressInteract() {
        if (!_active) { return; }
        if (currentInteractable != null) {
            currentInteractable.InteractPress(PlayerController.Instance);
        }
    }

    private void HoldInteract() {
        if (!_active) { return; }
        if (currentInteractable != null) {
            currentInteractable.InteractHold(PlayerController.Instance);
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
