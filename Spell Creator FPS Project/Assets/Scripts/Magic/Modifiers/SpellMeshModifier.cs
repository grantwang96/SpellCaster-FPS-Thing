using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Modifier/Spell Mesh Modifier")]
public class SpellMeshModifier : SpellModifier {

    [SerializeField] private Mesh _nextMesh;

    public override SpellStats SetupSpell(SpellStats stats) {
        return stats;
    }

    public override void SetupProjectile(MagicProjectile projectile) {
        projectile.SetParticleSystemMesh(_nextMesh);
    }
}
