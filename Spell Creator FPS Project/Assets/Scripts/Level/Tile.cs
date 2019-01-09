using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    // SOME NOTES:
    // This probably shouldn't permanently store the TileData for sake of RAM performance
    // This MIGHT store 3D array indices in case it needs them later on

    public TileData TileData;

    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Collider _collider;

    public void Initialize() {
        
    }
}
