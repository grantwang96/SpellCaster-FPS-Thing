using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpellCaster {

    Transform GunBarrel { get; set; }
    IDamageable Damageable { get; }
    CharacterBehaviour CharacterBehaviour { get; }
    SpellsInventory SpellsInventory { get; }
}
