using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MaterialOverride : MonoBehaviour {

    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private int _overrideValue;

	// Use this for initialization
	void Start () {
		for(int i = 0; i < _meshRenderer.materials.Length; i++) {
            _meshRenderer.materials[i].renderQueue = _overrideValue;
        }
	}
}
