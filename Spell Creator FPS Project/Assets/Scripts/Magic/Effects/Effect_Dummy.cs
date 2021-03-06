﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Effect/TestDummy")]
public class Effect_Dummy : Effect {

    [SerializeField] private float _upwardForce;

    public override void TriggerEffect(Damageable caster, float powerScale, List<Effect> effects = null) {
        Debug.Log(caster + " performed spell!");
    }

    public override void TriggerEffect(Damageable caster, float powerScale, Vector3 position, Damageable damageable = null, List<Effect> effects = null) {
        if (caster == damageable || damageable == null) { return; }
        Debug.Log(caster + " performed spell on " + damageable);
    }

    public override void TriggerEffect(Damageable caster, Vector3 velocity, float powerScale, Vector3 position, Damageable damageable = null, List<Effect> effects = null) {
        if (caster == damageable || damageable == null) { return; }
        Debug.Log(caster + " performed spell on " + damageable);
        velocity.y += _upwardForce;
        damageable.AddForce(velocity, _basePower);
    }
}
