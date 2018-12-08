using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageable : MonoBehaviour, IDamageable {

    [SerializeField] private int _health;
    public int Health { get { return _health; } }

    private bool _isDead;
    public bool IsDead { get { return _isDead; } }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TakeDamage(int damage) {
        _health -= damage;
        if(_health < 0) { Die(); }
    }

    public void Die() {

    }
}
