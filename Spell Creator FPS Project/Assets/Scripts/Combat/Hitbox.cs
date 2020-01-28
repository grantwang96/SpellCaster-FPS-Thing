using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private Damageable _owner;
    [SerializeField] private float _powerScale = 1f;

    private Effect[] _effects;
    private Vector3 _knockBackDir;

    public void Initialize(HitboxInfo info) {
        _effects = info.Effects;
        _knockBackDir = info.KnockBackDir;
    }

    void OnTriggerEnter(Collider other) {
        Damageable damageable = other.GetComponent<Damageable>();
        if (damageable != null && damageable != _owner) {
            TriggerEffects(damageable);
        }
    }

    private void TriggerEffects(Damageable target) {
        Vector3 direction = _owner.transform.TransformDirection(_knockBackDir);
        for (int i = 0; i < _effects.Length; i++) {
            _effects[i].TriggerEffect(_owner, direction, _powerScale, transform.position, target);
        }
    }
}

[System.Serializable]
public class HitboxInfo {
    public string Id;
    public Effect[] Effects;
    public Vector3 KnockBackDir;
}
