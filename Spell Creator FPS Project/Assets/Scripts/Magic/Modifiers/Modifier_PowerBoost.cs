using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Modifier/Power Boost")]
public class Modifier_PowerBoost : SpellModifier {

    [Range(1f, 10f)][SerializeField] private float _multiplier;
    [Range(0f, 1f)][SerializeField] private float _constant;

    public override void SetupProjectile(MagicProjectile projectile) {
        // visual effects?
    }

    public override SpellStats SetupSpell(SpellStats stats) {
        stats.PowerScale = (int)(stats.PowerScale * _multiplier);
        stats.PowerScale += _constant;
        return stats;
    }
}
