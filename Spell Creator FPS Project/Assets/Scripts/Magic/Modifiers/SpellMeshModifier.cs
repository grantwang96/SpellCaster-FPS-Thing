using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Modifier/Spell Mesh Modifier")]
public class SpellMeshModifier : SpellModifier {

    [SerializeField] private Mesh _nextMesh;

    public override void SetupSpell(Spell spell) {
        
    }

    public override void SetupProjectile(MagicProjectile projectile) {
        MeshFilter meshFilter = projectile.MeshFilter;
        meshFilter.mesh = _nextMesh;
    }
}
