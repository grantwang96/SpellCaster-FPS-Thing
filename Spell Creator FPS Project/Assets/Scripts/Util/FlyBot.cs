using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// For easy level traversing
/// </summary>
public class FlyBot : MonoBehaviour {

    private CharacterController _characterController;
    private Vector3 _velocity;
    [SerializeField] private float _speed;

    [SerializeField] private Vector3 lookRotation;
    [SerializeField] private float lookSpeed;
    [SerializeField] private float maxVerticalLookRotation;
    
    [SerializeField] private Transform _head;
    public Transform Head { get { return _head; } set { _head = value; } }

    // Use this for initialization
    void Start () {
        _characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {
        ProcessInputs();
        ProcessLookInput();
	}

    private void FixedUpdate() {
        _characterController.Move(transform.forward * _velocity.z + transform.right * _velocity.x);
    }

    private void ProcessInputs() {
        float leftRight = Input.GetAxis("Horizontal");
        float frontBack = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.Q)) {
            _velocity.y = -_speed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.Space)) {
            _velocity.y = _speed * Time.deltaTime;
        } else {
            _velocity.y = 0f;
        }

        _velocity.x = leftRight * _speed * Time.deltaTime;
        _velocity.z = frontBack * _speed * Time.deltaTime;
    }

    private void ProcessLookInput() {
        Vector3 lookInput = new Vector3();
        lookInput.x = Input.GetAxis("Mouse X");
        lookInput.y = Input.GetAxis("Mouse Y");
        lookRotation.x += lookInput.x * lookSpeed * Time.deltaTime;
        lookRotation.y -= lookInput.y * lookSpeed * Time.deltaTime;
        lookRotation.y = Mathf.Clamp(lookRotation.y, -maxVerticalLookRotation, maxVerticalLookRotation);
        
        Vector3 newHeadRotation = new Vector3(lookRotation.y, lookRotation.x, 0f);

        _head.localEulerAngles = newHeadRotation;
    }
}
