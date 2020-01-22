using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRadiusInteract : MonoBehaviour {

    [SerializeField] private SeparateColliderBroadcaster _triggerArea;

    // Use this for initialization
    void Start () {
        _triggerArea.TriggerEnter += TriggerAreaEnter;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void TriggerAreaEnter(Collider other) {
        ITriggerAreaInteractable interactable = other.GetComponent<ITriggerAreaInteractable>();
        if(interactable != null) {
            interactable.EnterInteractArea(PlayerController.Instance);
        }
    }
}
