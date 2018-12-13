using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDummyDamageable : MonoBehaviour, IDamageable {

    [SerializeField] private int _health;
    public int Health { get { return _health; } }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TakeDamage(int damage) {
        Debug.Log(name + " has received " + damage + " damage!");
        _health -= damage;
    }

    private void Die() {

    }
}
