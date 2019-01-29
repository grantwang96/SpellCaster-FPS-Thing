using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpellCaster {

    int Mana { get; }
    ActiveSpell ActiveSpell { get; }

    Transform GunBarrel { get; }
    IDamageable Damageable { get; }
    CharacterBehaviour CharacterBehaviour { get; }

    void PickUpSpell(Spell newSpell);
}
