using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellModifier : SpellComponent {
    
    public abstract void SetupSpell(Spell spell);
    public abstract void SetupProjectile(MagicProjectile projectile);
}
