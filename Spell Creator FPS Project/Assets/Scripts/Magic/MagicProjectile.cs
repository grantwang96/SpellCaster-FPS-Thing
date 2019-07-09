using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicProjectile : Projectile {
    
    [SerializeField] private ParticleSystem _mainParticleSystem;

    public MeshRenderer MeshRenderer { get; private set; }
    public ParticleSystem MainParticleSystem => _mainParticleSystem;
    public Spell Spell { get; private set; }
    public ISpellCaster SpellCaster { get; private set; }

    public override void ActivatePooledObject() {
        gameObject.SetActive(true);
    }

    public void InitializeMagic(ISpellCaster caster, Spell spell) {
        _owner = caster.Damageable;
        SpellCaster = caster;
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

    public void Initialize(ISpellCaster caster, Spell spell, Vector3 startPosition, Vector3 direction, float speed, float lifeTime) {
        SpellCaster = caster;
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

        MeshRenderer = _meshFilter.GetComponent<MeshRenderer>();
    }

    // Use this for initialization
    void Start () {
        _rigidBody.velocity = transform.forward * _speed;
        _previousPosition = _rigidBody.position;
	}

    protected override void OnHitDamageable(Damageable damageable) {
        Vector3 dir = damageable.Body.position - _rigidBody.position;
        float force = _rigidBody.velocity.magnitude * _rigidBody.mass;
        foreach (Effect effect in Spell.Effects) {
            effect?.TriggerEffect(SpellCaster.Damageable, dir.normalized * force, _power, transform.position, damageable);
        }
        Die();
    }

    public void SetParticleSystemMesh(Mesh newMesh) {
        var ParticleSystemShape = _mainParticleSystem.shape;
        ParticleSystemShape.mesh = newMesh;
    }
}
