using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// object that is thrown to deal damage/status effects
/// </summary>
public class Projectile : MonoBehaviour, PooledObject {

    public string PrefabId => name;
    [SerializeField] private bool _inUse;
    public bool InUse => _inUse;

    public string UniqueId { get; private set; }

    [SerializeField] protected Damageable _owner;
    [SerializeField] protected GameObject _onHitVFX;
    [SerializeField] protected Rigidbody _rigidBody;
    [SerializeField] protected Collider _collider;
    [SerializeField] protected MeshFilter _meshFilter;
    [SerializeField] protected Effect[] _effects;

    public MeshFilter MeshFilter { get { return _meshFilter; } }
    public Rigidbody Rigidbody { get { return _rigidBody; } }
    public Collider Collider { get { return _collider; } }

    protected Vector3 _previousPosition;
    [SerializeField] protected LayerMask _collisionMask;

    protected float _lifeTime;
    protected float _startTime;
    protected float _speed;
    protected float _powerScale;

    [SerializeField] protected bool _isLive; // can things collider with this projectile still?
    public bool IsLive => _isLive;

    private void Awake() {
        _rigidBody = GetComponent<Rigidbody>();
    }

    public void InitializeOwner(Damageable damageable) {
        _owner = damageable;
    }

    // initialize projectile's stats like power, gravity, and velocity
    public void InitializeProjectile(float powerScale, float lifeTime, Effect[] effects) {
        _powerScale= powerScale;
        _lifeTime = lifeTime;
        _effects = effects;
        _rigidBody.isKinematic = true;
        _rigidBody.velocity = Vector3.zero;
        _collider.enabled = false;
    }

    public virtual void FireProjectile(bool useGravity, Vector3 vector) {
        _isLive = true;
        transform.SetParent(PooledObjectManager.Instance.Transform);
        transform.forward = vector;
        _rigidBody.isKinematic = false;
        _rigidBody.useGravity = useGravity;
        _rigidBody.AddForce(vector, ForceMode.VelocityChange);
        _collider.enabled = true;
        _startTime = Time.time;
    }
	
	protected virtual void Update () {
        if(_rigidBody.velocity != Vector3.zero) {
            transform.forward = _rigidBody.velocity;
        }
        if (_isLive && Time.time - _startTime >= _lifeTime) {
            DeactivatePooledObject();
        }
    }

    protected virtual void FixedUpdate() {
        _previousPosition = _rigidBody.position;
    }

    protected virtual void LateUpdate() {
        CheckClipping();
    }

    protected virtual void OnTriggerEnter(Collider coll) {
        Damageable dam = coll.GetComponent<Damageable>();
        if (dam == _owner) {
            return;
        }
        if (dam == null) {
            OnHitCollider(coll);
            return;
        }
        OnHitDamageable(transform.position, dam);
        // if it collides with a wall
    }

    protected virtual void OnHitCollider(Collider coll) {
        // handle non damageable situation
        Vector3 dir = coll.transform.position - transform.position;
        dir = dir.normalized;
        float force = _rigidBody.velocity.magnitude * _rigidBody.mass;
        if(coll.attachedRigidbody != null) {
            coll.attachedRigidbody.AddForce(dir * force, ForceMode.Impulse);
        }

        for (int i = 0; i < _effects.Length; i++) {
            _effects[i].TriggerEffect(_owner, _powerScale, transform.position, coll);
        }
        Die();
    }

    // what happens when this collides with something?
    protected virtual void OnHitDamageable(Vector3 hitPoint, Damageable damageable) {
        // apply effects here
        for(int i = 0; i < _effects.Length; i++) {
            _effects[i].TriggerEffect(_owner, _powerScale, hitPoint, damageable);
        }
        Die();
    }

    protected void CheckClipping() {
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
        if (Physics.Raycast(_previousPosition, dir, out hit, Vector3.Distance(_previousPosition, _rigidBody.position), _collisionMask)) {
            if (hit.collider == other) {
                return hit.point;
            }
        }
        return _rigidBody.position;
    }

    protected virtual IEnumerator Effects() {
        // start effects
        // wait for effects to finished
        yield return new WaitForEndOfFrame();
        // deactivate and return object to pool
        DeactivatePooledObject();
    }

    public virtual void ActivatePooledObject(string uniqueId = "") {
        gameObject.SetActive(true);
        UniqueId = uniqueId;
    }

    public void DeactivatePooledObject() {
        gameObject.SetActive(false);
        // transform.SetParent(ObjectPool.Instance?.transform);
        PooledObjectManager.Instance.ReturnPooledObject(PrefabId, this);
    }

    protected void Die() {
        _isLive = false;
        StartCoroutine(Effects());
    }
}
