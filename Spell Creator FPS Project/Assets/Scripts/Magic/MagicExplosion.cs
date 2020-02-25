using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicExplosion : MonoBehaviour {

    private List<Effect> _effects;
    private float _radius;
    private float _force;
    private float _totalTime;
    private float _time;
    private float _powerScale;
    private Damageable _owner;

    [SerializeField] private SphereCollider _collider;
    [SerializeField] private MeshFilter _meshFilter;

    private List<Collider> _ignoredColliders = new List<Collider>();

    public void Initialize(List<Effect> effects, float radius, float force, float time, float powerScale, Damageable owner, Damageable damageable = null) {
        _effects = effects;
        _radius = radius;
        _totalTime = time;
        _time = 0f;
        _powerScale = powerScale;
        _owner = owner;

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
        Vector3 dir = (damageable.Root.position - transform.position).normalized;
        foreach (Effect effect in _effects) {
            effect?.TriggerEffect(_owner, dir * _force, _powerScale, transform.position, damageable, _effects);
        }
    }
}
