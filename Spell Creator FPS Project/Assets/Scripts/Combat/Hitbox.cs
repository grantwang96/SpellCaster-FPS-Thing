using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private GameObject _ownerGO;
    [SerializeField] private float _powerScale = 1f;

    private Damageable _owner;
    private HitboxInfo _info;
    private HitData _hitData;

    public void Initialize(HitboxInfo info) {
        if(_owner == null) {
            _owner = _ownerGO.GetComponent<Damageable>();
        }
        _info = info;
        _hitData = new HitData(_info, _owner, _powerScale, transform.position);
    }

    private void OnTriggerEnter(Collider other) {
        Hurtbox hurtBox = other.GetComponent<Hurtbox>();
        if(hurtBox != null) {
            hurtBox.Hit(_hitData);
        }
    }
}

[System.Serializable]
public class HitboxInfo {
    public string Id;
    public Effect[] Effects;
    public Vector3 KnockBackDir;
}

public class HitData {

    public readonly Effect[] Effects;
    public readonly Vector3 KnockBackDir;
    public readonly Damageable Owner;
    public readonly Vector3 Origin;
    public readonly float PowerScale;

    public HitData(HitboxInfo info, Damageable owner, float powerScale, Vector3 origin) {
        Effects = info.Effects;
        KnockBackDir = info.KnockBackDir;
        Owner = owner;
        PowerScale = powerScale;
        Origin = origin;
    }

    public HitData(Effect[] effects, Vector3 knockBackDir, Damageable owner, float powerScale, Vector3 origin) {
        Effects = effects;
        KnockBackDir = knockBackDir;
        Owner = owner;
        PowerScale = powerScale;
        Origin = origin;
    }

    public void TriggerEffects(Damageable target) {
        Vector3 direction = Owner.Body.TransformDirection(KnockBackDir);
        for (int i = 0; i < Effects.Length; i++) {
            Effects[i].TriggerEffect(Owner, direction, PowerScale, Origin, target);
        }
    }
}
