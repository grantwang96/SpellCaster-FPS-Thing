using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RecoveryOrbType {
    Health, Mana
}

public class RecoveryOrb : PooledObject {

    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Collider _colliderInner;
    [SerializeField] private Collider _colliderOuter;
    [SerializeField] private SeparateColliderBroadcaster _triggerArea;
    [SerializeField] private float _flySpeed;
    [SerializeField] private float _spawnSpread;
    [SerializeField] private float _spawnForce;

    private RecoveryOrbType _type;
    private bool _interactable = true;

    public void Initialize(RecoveryOrbType type) {
        _type = type;
        _interactable = true;
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
        _colliderInner.enabled = true;
        _colliderOuter.enabled = true;

        Spawn();
    }

    private void Spawn() {
        Vector3 offset = Random.insideUnitCircle * _spawnSpread;
        offset = new Vector3(offset.x, 0f, offset.y);
        Vector3 dir = Vector3.up + offset;
        _rigidbody.AddForce(dir.normalized * _spawnForce, ForceMode.Impulse);
    }

	// Use this for initialization
	void Start () {
        _triggerArea.TriggerEnter += OnRecoveryOrbAreaEntered;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDestroy() {
        _triggerArea.TriggerEnter -= OnRecoveryOrbAreaEntered;
    }

    private void OnRecoveryOrbAreaEntered(Collider other) {
        if (!_interactable) { return; }
        if (_type == RecoveryOrbType.Health) {
            Damageable dam = other.GetComponent<Damageable>();
            if (dam == GameplayController.Instance.Damageable) {
                TryHeal(dam);
            }
        } else {
            ISpellCaster caster = other.GetComponent<ISpellCaster>();
            if (caster == GameplayController.Instance.PlayerCombat) {
                TryRestoreMana(caster);
            }
        }
    }

    private void TryHeal(Damageable dam) {
        dam.TakeDamage(-GameplayValues.Level.HealthOrbValue);
        StartCoroutine(FlyToTarget(dam.Body));
    }

    private void TryRestoreMana(ISpellCaster caster) {
        caster.RecoverMana(GameplayValues.Level.ManaOrbValue);
        StartCoroutine(FlyToTarget(caster.Damageable.Body));
    }

    private IEnumerator FlyToTarget(Transform target) {
        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;
        _colliderInner.enabled = false;
        _colliderOuter.enabled = false;
        _interactable = false;
        float time = 0f;
        while(time < 1f) {
            time += Time.deltaTime * _flySpeed;
            transform.position = Vector3.Lerp(transform.position, target.position, time);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        DeactivatePooledObject();
    }

    public override void ActivatePooledObject() {
        base.ActivatePooledObject();
        gameObject.SetActive(true);
    }

    public override void DeactivatePooledObject() {
        gameObject.SetActive(false);
        transform.SetParent(ObjectPool.Instance?.transform);
        ObjectPool.Instance.ReturnUsedPooledObject(PrefabId, this);
        _interactable = false;
    }
}
