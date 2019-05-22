
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpellCaster {

    int Mana { get; }
    ActiveSpell ActiveSpell { get; }

    Transform GunBarrel { get; }
    Damageable Damageable { get; }
    CharacterBehaviour CharacterBehaviour { get; }

    void PickUpSpell(Spell newSpell);
    void RecoverMana(int mana);
}
