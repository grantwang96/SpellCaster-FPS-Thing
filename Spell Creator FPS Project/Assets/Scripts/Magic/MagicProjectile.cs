using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicProjectile : MonoBehaviour {

    public Spell_Effect[] Effects { get; private set; }
    public ISpellCaster spellCaster { get; private set; }

    private Rigidbody _rigidBody;

    private float _lifeTime;
    private float _startTime;
    private float _speed;

    private int _power;

    public void InitializeMagic(ISpellCaster caster, Spell_Effect[] effects) {
        spellCaster = caster;
        Effects = effects;
    }

    public void InitializePosition(Vector3 startPosition, Vector3 direction) {

        transform.position = startPosition;
        transform.forward = direction;
    }

    public void InitializeStats(float speed, float lifeTime, int power) {
        _speed = speed;
        _lifeTime = lifeTime;
        _power = power;
    }

    public void Initialize(ISpellCaster caster, Spell_Effect[] effects, Vector3 startPosition, Vector3 direction, float speed, float lifeTime) {
        spellCaster = caster;
        Effects = effects;
        transform.position = startPosition;
        transform.forward = direction;
        _speed = speed;
        _lifeTime = lifeTime;
    }

    private void Awake() {
        _rigidBody = GetComponent<Rigidbody>();
    }

    // Use this for initialization
    void Start () {
        _startTime = Time.time;
        _rigidBody.velocity = transform.forward * _speed;
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time - _startTime >= _lifeTime) {
            Die();
        }
	}

    private void Die() {
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision) {
        IDamageable damageable = collision.transform.GetComponent<IDamageable>();
        if (damageable != null) {
            if (damageable == spellCaster.Damageable) { return; }
            ApplyEffects(damageable);
        }
        Die();
    }

    private void OnTriggerEnter(Collider other) {
        IDamageable damageable = other.GetComponent<IDamageable>();
        Vector3 dir = other.transform.position - _rigidBody.position;
        if (damageable != null) {
            if (damageable == spellCaster.Damageable) { return; }
            ApplyEffects(damageable);
        } else if(other.attachedRigidbody != null) {
            other.attachedRigidbody.AddForce(dir.normalized * _speed * _rigidBody.mass, ForceMode.Impulse);
        }
        Die();
    }

    private void ApplyEffects(IDamageable damageable) {
        foreach(Spell_Effect effect in Effects) {
            effect.TriggerEffect(damageable, spellCaster, _rigidBody.velocity);
        }
    }
}
