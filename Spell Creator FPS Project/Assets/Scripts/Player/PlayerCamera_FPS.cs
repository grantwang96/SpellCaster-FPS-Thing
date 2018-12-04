using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera_FPS : MonoBehaviour {

    [SerializeField] private float horizontalBodyRotation = 0f;
    [SerializeField] private float verticalHeadRotation = 0f;

    [SerializeField] private float lookSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        ProcessLookInput();
	}

    private void ProcessLookInput() {
        Transform head = GameplayController.Instance.Head;
        Transform body = GameplayController.Instance.BodyTransform;

        Vector3 lookInput = GameplayController.Instance.lookVector;

        horizontalBodyRotation += lookInput.x * lookSpeed * Time.deltaTime;
        verticalHeadRotation -= lookInput.y * lookSpeed * Time.deltaTime;
        verticalHeadRotation = Mathf.Clamp(verticalHeadRotation, -80f, 80f);

        Vector3 newBodyRotation = new Vector3(0f, horizontalBodyRotation, 0f);
        Vector3 newHeadRotation = new Vector3(verticalHeadRotation, 0f, 0f);

        head.localEulerAngles = newHeadRotation;
        body.localEulerAngles = newBodyRotation;
    }
}
