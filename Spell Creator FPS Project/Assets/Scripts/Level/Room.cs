using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

    [SerializeField] protected string _roomId;
    public string RoomId { get { return _roomId; } }

    [SerializeField] protected List<EnemySpawn> _enemySpawn = new List<EnemySpawn>();

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

    private void OnTriggerEnter(Collider other) {
        CharacterBehaviour character = other.GetComponent<CharacterBehaviour>();
        if(character != null) {
            if(character == PlayerController.Instance) {
                OnPlayerEnter();
            }
        }
    }

    private void OnPlayerEnter() {
        Debug.Log($"Player has entered room {_roomId}!");
        
    }

    private void SpawnEnemies() {
        for(int i = 0; i < _enemySpawn.Count; i++) {
            _enemySpawn[i].SpawnNPC();
        }
    }
}