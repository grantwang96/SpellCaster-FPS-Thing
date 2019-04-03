using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicExplosion : MonoBehaviour {

    private Spell _spell;
    private float _radius;
    private float _force;
    private float _totalTime;
    private float _time;
    private int _power;
    private ISpellCaster _spellCaster;

    [SerializeField] private SphereCollider _collider;
    [SerializeField] private MeshFilter _meshFilter;

    private List<Collider> _ignoredColliders = new List<Collider>();

    public void Initialize(Spell spell, float radius, float force, float time, int power, ISpellCaster spellCaster, Damageable damageable = null) {
        _spell = spell;
        _radius = radius;
        _totalTime = time;
        _time = 0f;
        _power = power;
        _spellCaster = spellCaster;

        transform.localScale = Vector3.zero;
    }
	
	private void FixedUpdate () {
		if(_time >= _totalTime) {
            Destroy(gameObject);
        }
        _time += Time.deltaTime;
        transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * _radius, _time / _totalTime);
    }

    private void OnTriggerEnter(Collider other) {
        if (_ignoredColliders.Contains(other)) {
            return;
        }
        Damageable damageable = other.GetComponent<Damageable>();
        if(damageable != null) {
            ApplyEffects(damageable);
            return;
        } else if(other.attachedRigidbody != null) {
            other.attachedRigidbody.AddExplosionForce(_force, transform.position, _radius);
        }
    }

    private void ApplyEffects(Damageable damageable) {
        Vector3 dir = (damageable.transform.position - transform.position).normalized;
        foreach (Spell_Effect effect in _spell.Effects) {
            effect?.TriggerEffect(_spellCaster, dir * _force, _power, _spell, transform.position, damageable);
        }
    }
}
