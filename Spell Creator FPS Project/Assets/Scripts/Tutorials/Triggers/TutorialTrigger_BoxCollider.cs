using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger_BoxCollider : MonoBehaviour {

    [SerializeField] private string _tutorialTriggerId;
    [SerializeField] private bool _disableOnTrigger;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other) {
        // check if player walked thru here
        CharacterBehaviour character = other.GetComponent<CharacterBehaviour>();
        if(character == null || character != PlayerController.Instance) {
            return;
        }
        // fire tutorial trigger
        TutorialManager.Instance.FireTutorialTrigger(_tutorialTriggerId);
        gameObject.SetActive(!_disableOnTrigger);
    }
}