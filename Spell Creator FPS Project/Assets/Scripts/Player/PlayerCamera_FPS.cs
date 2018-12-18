using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera_FPS : MonoBehaviour {

    [SerializeField] private Vector3 lookRotation;
    [SerializeField] private float lookSpeed;
    [SerializeField] private float maxVerticalLookRotation;

    [SerializeField] private Transform _body;
    public Transform Body { get { return _body; } set { _body = value; } }
    [SerializeField] private Transform _head;
    public Transform Head { get { return _head; } set { _head = value; } }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        ProcessLookInput();
	}

    private void ProcessLookInput() {
        Vector3 lookInput = GameplayController.Instance.LookVector;
        lookRotation.x += lookInput.x * lookSpeed * Time.deltaTime;
        lookRotation.y -= lookInput.y * lookSpeed * Time.deltaTime;
        lookRotation.y = Mathf.Clamp(lookRotation.y, -maxVerticalLookRotation, maxVerticalLookRotation);

        Vector3 newBodyRotation = new Vector3(0f, lookRotation.x, 0f);
        Vector3 newHeadRotation = new Vector3(lookRotation.y, 0f, 0f);

        _head.localEulerAngles = newHeadRotation;
        _body.localEulerAngles = newBodyRotation;
    }
}
