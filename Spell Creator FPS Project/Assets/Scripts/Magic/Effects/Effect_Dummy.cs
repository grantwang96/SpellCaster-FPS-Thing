using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Effect/TestDummy")]
public class Effect_Dummy : Spell_Effect {

    public override void TriggerEffect(IDamageable damageable, ISpellCaster caster) {
        Debug.Log(caster.Damageable + " performed spell on " + damageable);
    }
}
