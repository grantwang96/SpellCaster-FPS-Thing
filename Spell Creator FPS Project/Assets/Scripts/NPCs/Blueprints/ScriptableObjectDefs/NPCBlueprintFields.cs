using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract partial class NPCBlueprint : ScriptableObject {

    [SerializeField] protected int _totalHealth;
    public int TotalHealth { get { return _totalHealth; } }
    [SerializeField] protected float _walkSpeed;
    public float WalkSpeed { get { return _walkSpeed; } }
    [SerializeField] protected float _runSpeed;
    public float RunSpeed { get { return _runSpeed; } }

    [SerializeField] private float _idleTimeMinimum;
    [SerializeField] private float _idleTimeMaximum;
    public float GetNewIdleTime { get { return Random.Range(_idleTimeMinimum, _idleTimeMaximum); } }

    [SerializeField] protected float _visionRange;
    public float VisionRange { get { return _visionRange; } }
    [SerializeField] protected float _visionAngle;
    public float VisionAngle { get { return _visionAngle; } }
    [SerializeField] protected LayerMask _visionMask;
    public LayerMask VisionMask { get { return _visionMask; } }
}
