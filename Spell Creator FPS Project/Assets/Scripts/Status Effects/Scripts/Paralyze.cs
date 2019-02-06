using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Status Effect/Paralyze")]
public class Paralyze : StatusEffect {

    public override void OnAddEffect(Damageable damageable, int power) {
        CharacterMoveController characterMove = damageable.GetComponent<CharacterMoveController>();
        if (characterMove != null) {
            characterMove.enabled = false;
        }
    }

    public override void OnRemoveEffect(Damageable damageable) {
        CharacterMoveController characterMove = damageable.GetComponent<CharacterMoveController>();
        if (characterMove != null) {
            characterMove.enabled = true;
        }
    }
}
