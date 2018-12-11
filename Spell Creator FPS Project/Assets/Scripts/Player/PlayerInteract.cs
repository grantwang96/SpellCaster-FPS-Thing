using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour {

    [SerializeField] private float interactDistance;
    [SerializeField] private LayerMask interactLayers;

    private Transform head;

    private IInteractable currentInteractable;

    private void OnEnable() {
        GameplayController.Instance.OnInteractPressed += AttemptInteract;
        GameplayController.Instance.OnInteractHeld += AttemptInteract;
    }

    private void OnDisable() {
        GameplayController.Instance.OnInteractPressed += AttemptInteract;
        GameplayController.Instance.OnInteractHeld += AttemptInteract;
    }

    // Use this for initialization
    void Start () {
        head = GameplayController.Instance.Head;
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

    private void AttemptInteract() {
        if(currentInteractable != null) {
            currentInteractable.Interact(GameplayController.Instance);
        }
    }
}
