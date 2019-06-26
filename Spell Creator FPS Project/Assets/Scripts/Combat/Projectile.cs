﻿using System.Collections;
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
    [SerializeField] protected bool _isLive; // can things collider with this projectile still?
    public bool IsLive => _isLive;

    private void Awake() {
        _rigidBody = GetComponent<Rigidbody>();
    }

    public void InitializeOwner(Damageable damageable) {
        _owner = damageable;
    }

    // initialize projectile's stats like power, gravity, and velocity
    public void InitializeProjectile(int power, float lifeTime, Effect[] effects) {
        _power = power;
        _lifeTime = lifeTime;
        _effects = effects;
    }

    public virtual void FireProjectile(bool useGravity, Vector3 vector) {
        _isLive = true;
        transform.SetParent(ObjectPool.Instance.transform);
        transform.forward = vector;
        _rigidBody.useGravity = useGravity;
        _rigidBody.AddForce(vector, ForceMode.VelocityChange);
        _collider.enabled = true;
        _startTime = Time.time;
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
        Damageable dam = coll.GetComponent<Damageable>();
        if (dam == _owner) {
            return;
        }
        if (dam == null) {
            OnHitCollider(coll);
            return;
        }
        OnHitDamageable(dam);
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
            _effects[i].TriggerEffect(_owner, _power, transform.position, coll);
        }
        Die();
    }

    // what happens when this collides with something?
    protected virtual void OnHitDamageable(Damageable damageable) {
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
    }

    private void Die() {
        _isLive = false;
        StartCoroutine(Effects());
    }
}