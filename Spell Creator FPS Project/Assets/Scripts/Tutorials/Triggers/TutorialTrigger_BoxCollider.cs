using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger_BoxCollider : MonoBehaviour {

    [SerializeField] private string _tutorialTriggerKey;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other) {
        // check if player walked thru here
        // fire tutorial trigger
    }
}
