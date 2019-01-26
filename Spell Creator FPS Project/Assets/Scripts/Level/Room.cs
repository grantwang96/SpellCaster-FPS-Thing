using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

    [SerializeField] protected string _roomId;
    public string RoomId { get { return _roomId; } }

    [SerializeField] protected MeshFilter _meshFilter;
    [SerializeField] protected BoxCollider _boxCollider;

    void Awake() {
        _boxCollider = GetComponent<BoxCollider>();
    }

    void Start() {  
        /*
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];
        for(int i = 0; i < meshFilters.Length; i++) {
            combineInstances[i].mesh = meshFilters[i].mesh;
            combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        _meshFilter.mesh = new Mesh();
        _meshFilter.mesh.CombineMeshes(combineInstances);
        gameObject.SetActive(true);
        */
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log(other.name + " has stepped into Room: " + _roomId);
    }
}
