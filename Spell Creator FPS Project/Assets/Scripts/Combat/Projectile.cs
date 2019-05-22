using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// object that is thrown to deal damage/status effects
/// </summary>
public class Projectile : PooledObject {

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

    protected int _power;
    protected bool _isLive; // can things collider with this projectile still?
    public bool IsLive => _isLive;

    private void Awake() {
        _rigidBody = GetComponent<Rigidbody>();
    }

    public void InitializeOwner(Damageable damageable) {
        _owner = damageable;
    }

    // initialize projectile's stats like power, gravity, and velocity
    public void FireProjectile(int power, bool useGravity, Vector3 vector, float lifeTime, Effect[] effects) {
        _isLive = true;
        _power = power;
        transform.SetParent(ObjectPool.Instance.transform);
        transform.forward = vector;
        _rigidBody.useGravity = useGravity;
        _rigidBody.velocity = vector;
        _lifeTime = lifeTime;
        _collider.enabled = true;
        _startTime = Time.time;
        _effects = effects;
    }
	
	protected virtual void Update () {
        transform.forward = _rigidBody.velocity;
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
        Debug.Log(coll.gameObject);
        Debug.Log(_owner);
        Damageable dam = coll.GetComponent<Damageable>();
        if(dam == _owner) {
            return;
        }
        OnHit(dam);
        // if it collides with a wall
    }

    // what happens when this collides with something?
    protected virtual void OnHit(Damageable damageable = null) {
        if(damageable != null) {
            // handle effects
        }
        _isLive = false;
        // apply effects here
        for(int i = 0; i < _effects.Length; i++) {
            _effects[i].TriggerEffect(_owner, _power, damageable);
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

    private IEnumerator Effects() {
        // start effects
        // wait for effects to finished
        yield return new WaitForEndOfFrame();
        // deactivate and return object to pool
        DeactivatePooledObject();
    }

    public override void ActivatePooledObject() {
        gameObject.SetActive(true);
    }

    public override void DeactivatePooledObject() {
        gameObject.SetActive(false);
        transform.SetParent(ObjectPool.Instance?.transform);
        ObjectPool.Instance.ReturnUsedPooledObject(PrefabId, this);
        _isLive = false;
    }

    private void Die() {
        StartCoroutine(Effects());
    }
}
