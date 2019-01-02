using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicProjectile : MonoBehaviour {

    public Spell_Effect[] Effects { get; private set; }
    public ISpellCaster spellCaster { get; private set; }

    [SerializeField] private MeshFilter _meshFilter;
    public MeshFilter MeshFilter { get { return _meshFilter; } }
    private Rigidbody _rigidBody;
    public Rigidbody Rigidbody { get { return _rigidBody; } }
    private Collider _collider;
    public Collider Collider { get { return _collider; } }

    private Vector3 _previousPosition;
    [SerializeField] private LayerMask _collisionMask;

    private float _lifeTime;
    private float _startTime;
    private float _speed;

    private int _power;

    public void InitializeMagic(ISpellCaster caster, Spell_Effect[] effects) {
        spellCaster = caster;
        Effects = effects;
    }

    public void InitializeMagicModifiers(SpellModifier[] modifiers) {
        for(int i = 0; i < modifiers.Length; i++) {
            modifiers[i].SetupProjectile(this);
        }
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
        _collider = GetComponent<Collider>();
    }

    // Use this for initialization
    void Start () {
        _startTime = Time.time;
        _rigidBody.velocity = transform.forward * _speed;
        _previousPosition = _rigidBody.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time - _startTime >= _lifeTime) {
            Die();
        }
	}

    private void FixedUpdate() {
        _previousPosition = _rigidBody.position;
    }

    private void LateUpdate() {
        CheckClipping();
    }

    private void Die() {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        IDamageable damageable = other.transform.GetComponent<IDamageable>();
        Vector3 dir = other.transform.position - _rigidBody.position;
        float force = _rigidBody.velocity.magnitude * _rigidBody.mass;

        if (damageable != null) {
            if (damageable == spellCaster.Damageable) { return; }
            ApplyEffects(damageable, dir.normalized * force);
        } else if(other.attachedRigidbody != null) {
            other.attachedRigidbody.AddForce(dir.normalized * force, ForceMode.Impulse);
        } else {
            ApplyEffects();
        }

        Die();
    }

    private void CheckClipping() {
        RaycastHit hit;
        Vector3 dir = _rigidBody.position - _previousPosition;
        if (Physics.Raycast(_previousPosition, dir, out hit, Vector3.Distance(_previousPosition, _rigidBody.position), _collisionMask)) {
            _rigidBody.position = hit.point;
            _collider.SendMessage("OnTriggerEnter", hit.collider);
        }
    }

    private Vector3 CalculateContactPosition(Collider other) {
        RaycastHit hit;
        Vector3 dir = _rigidBody.position - _previousPosition;
        if(Physics.Raycast(_previousPosition, dir, out hit, Vector3.Distance(_previousPosition, _rigidBody.position), _collisionMask)) {
            if (hit.collider == other) {
                return hit.point;
            }
        }
        return _rigidBody.position;
    }

    private void ApplyEffects() {
        foreach(Spell_Effect effect in Effects) {
            effect.TriggerEffect(spellCaster, _power);
        }
    }

    private void ApplyEffects(IDamageable damageable, Vector3 force) {
        foreach (Spell_Effect effect in Effects) {
            effect.TriggerEffect(damageable, spellCaster, force, _power);
        }
    }
}
