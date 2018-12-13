using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicProjectile : MonoBehaviour {

    public Spell_Effect[] Effects { get; private set; }
    public ISpellCaster spellCaster { get; private set; }

    private Rigidbody rigidbody;

    private float _lifeTime;
    private float _startTime;
    private float _speed;

    public void Initialize(ISpellCaster caster, Spell_Effect[] effects, Vector3 startPosition, Vector3 direction, float speed) {
        spellCaster = caster;
        Effects = effects;
        transform.position = startPosition;
        transform.forward = direction;
        _speed = speed;
    }

    private void Awake() {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Use this for initialization
    void Start () {
		
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

    private void OnTriggerEnter(Collider other) {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if(damageable != null) {
            ApplyEffects(damageable);
        }
        Die();
    }

    private void ApplyEffects(IDamageable damageable) {
        foreach(Spell_Effect effect in Effects) {
            effect.TriggerEffect(damageable, spellCaster);
        }
    }
}
