using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtboxController : MonoBehaviour
{
    [SerializeField] private Damageable _owner;
    [SerializeField] private List<Hurtbox> _hurtBoxGOs = new List<Hurtbox>();

    private Dictionary<string, Hurtbox> _hurtBoxes = new Dictionary<string, Hurtbox>();

    private bool _hit;
    private float _damageScale;
    private int _currentPriority;
    private HitData _hitData;

    private void Awake() {
        RegisterHurtBoxes();
    }

    private void OnDestroy() {
        UnregisterHurtBoxes();
    }

    private void RegisterHurtBoxes() {
        for(int i = 0; i < _hurtBoxGOs.Count; i++) {
            _hurtBoxes.Add(_hurtBoxGOs[i].name, _hurtBoxGOs[i]);
            _hurtBoxGOs[i].OnHit += OnHit;
        }
    }

    private void UnregisterHurtBoxes() {
        for(int i = 0; i < _hurtBoxGOs.Count; i++) {
            _hurtBoxGOs[i].OnHit -= OnHit;
        }
        _hurtBoxes.Clear();
    }

    private void OnHit(HitData hitData, float damageScale, int priority) {

        // calculate resistances

        _hit = true;
        _damageScale = damageScale;
        _currentPriority = priority;
        _hitData = hitData;
    }

    private void LateUpdate() {
        if (_hit) {
            ProcesssHit();
        }
    }

    private void ProcesssHit() {
        // actually trigger the effects here
        _hitData?.TriggerEffects(_owner);
        _hit = false;
    }


    private void TriggerEffects() {
        Vector3 direction = _hitData.Owner.transform.TransformDirection(_hitData.KnockBackDir);
        Effect[] effects = _hitData.Effects;
        for (int i = 0; i < effects.Length; i++) {
            effects[i].TriggerEffect(_hitData.Owner, direction, _hitData.PowerScale * _damageScale, _hitData.Origin, _owner);
        }
    }
}
