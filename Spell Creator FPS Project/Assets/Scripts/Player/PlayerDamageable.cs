using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageable : CharacterDamageable {

    protected override void Die() {
        _health = 0;
        base.Die();
    }
}
