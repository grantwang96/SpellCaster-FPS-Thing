using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpellCaster {

    int mana { get; }
    ActiveSpell ActiveSpell { get; }

    Transform GunBarrel { get; }
    IDamageable Damageable { get; }
    CharacterBehaviour CharacterBehaviour { get; }
    SpellsInventory SpellsInventory { get; }
}
