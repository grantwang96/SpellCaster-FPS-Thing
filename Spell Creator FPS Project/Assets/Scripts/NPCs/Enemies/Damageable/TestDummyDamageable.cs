using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDummyDamageable : MonoBehaviour, IDamageable {

    [SerializeField] private int _health;
    public int Health { get { return _health; } }

    CharacterController characterController;
    CharacterMoveController moveController;
    Vector3 movementVelocity = Vector3.zero;

    Coroutine externalForces;

	// Use this for initialization
	void Start () {
        characterController = GetComponent<CharacterController>();
        moveController = GetComponent<CharacterMoveController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TakeDamage(int damage) {
        Debug.Log(name + " has received " + damage + " damage!");
        _health -= damage;
    }

    public void TakeDamage(int damage, Vector3 velocity) {
        TakeDamage(damage);
        AddForce(velocity);
    }

    private void Die() {

    }

    public void AddForce(Vector3 velocity) {
        moveController.AddForce(velocity);
    }
}
