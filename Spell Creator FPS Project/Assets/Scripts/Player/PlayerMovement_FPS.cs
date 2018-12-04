using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement_FPS : MonoBehaviour {

    [SerializeField] private float speed;
    [SerializeField] private float lookSpeed;
    [SerializeField] private float jumpForce;

    [SerializeField] private Vector3 moveVector;

    private CharacterController characterController;

    private void Awake() {
        characterController = GetComponent<CharacterController>();
    }

    // Use this for initialization
    void Start () {

	}

    private void OnDisable() {
        
    }

    // Update is called once per frame
    void Update () {
        ProcessGravity();

        characterController.Move(moveVector * Time.deltaTime);
	}
    
    private void ProcessGravity() {
        if(moveVector.y > Physics.gravity.y) {
            moveVector.y += Time.deltaTime * Physics.gravity.y;
        }
    }

    public void ProcessWalkInput(Vector3 inputVector) {
        Vector3 newMoveVector = new Vector3();
        newMoveVector += transform.right * inputVector.x;
        newMoveVector += transform.forward * inputVector.z;
        moveVector.x = newMoveVector.x * speed;
        moveVector.z = newMoveVector.z * speed;
    }

    public void OnJump() {
        if (characterController.isGrounded) {
            moveVector.y = jumpForce;
        }
    }
}
