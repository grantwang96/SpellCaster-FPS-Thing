using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Modifier/Power Boost")]
public class Modifier_PowerBoost : SpellModifier {

    [Range(1f, 10f)][SerializeField] private float _multiplier;
    [SerializeField] private int _constant;

    public override void SetupProjectile(MagicProjectile projectile) {
        // visual effects?
    }

    public override SpellStats SetupSpell(SpellStats stats) {
        stats.Power = (int)(stats.Power * _multiplier);
        stats.Power += _constant;
        return stats;
    }
}
