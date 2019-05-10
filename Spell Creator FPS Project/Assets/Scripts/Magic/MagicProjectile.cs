using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicProjectile : Projectile {

    public Spell Spell { get; private set; }
    public ISpellCaster spellCaster { get; private set; }

    public override void ActivatePooledObject() {
        gameObject.SetActive(true);
    }

    public void InitializeMagic(ISpellCaster caster, Spell spell) {
        spellCaster = caster;
        Spell = spell;
        InitializeMagicModifiers(Spell.SpellModifiers);
    }

    private void InitializeMagicModifiers(SpellModifier[] modifiers) {
        for(int i = 0; i < modifiers.Length; i++) {
            modifiers[i]?.SetupProjectile(this);
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

    public void Initialize(ISpellCaster caster, Spell spell, Vector3 startPosition, Vector3 direction, float speed, float lifeTime) {
        spellCaster = caster;
        Spell = spell;
        transform.position = startPosition;
        transform.forward = direction;
        _speed = speed;
        _lifeTime = lifeTime;
        InitializeMagicModifiers(Spell.SpellModifiers);
    }

    private void Awake() {
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    // Use this for initialization
    void Start () {
        _isLive = true; 
        _startTime = Time.time;
        _rigidBody.velocity = transform.forward * _speed;
        _previousPosition = _rigidBody.position;
	}

    protected override void OnTriggerEnter(Collider other) {
        Damageable damageable = other.transform.GetComponent<Damageable>();
        Vector3 dir = other.transform.position - _rigidBody.position;
        float force = _rigidBody.velocity.magnitude * _rigidBody.mass;

        if (damageable != null) {
            if (damageable == spellCaster.Damageable) { return; }
            ApplyEffects(dir.normalized * force, damageable);
            DeactivatePooledObject();
            return;
        } else if(other.attachedRigidbody != null) {
            other.attachedRigidbody.AddForce(dir.normalized * force, ForceMode.Impulse);
        }
        ApplyEffects(dir.normalized * force);

        DeactivatePooledObject();
    }

    private void ApplyEffects(Vector3 force, Damageable damageable = null) {
        foreach (Effect effect in Spell.Effects) {
            effect?.TriggerEffect(spellCaster.Damageable, force, _power, transform.position, damageable);
        }
    }
}
