using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {

    public static ObjectPool Instance { get; private set; }
    [SerializeField] private GameObject[] _poolPrefabs;

    private Dictionary<string, List<ObjectPool>> _objectPools = new Dictionary<string, List<ObjectPool>>();

	private void Awake() {
        Instance = this;
    }

    private void InitializePooledObjects() {
        for (int i = 0; i < _poolPrefabs.Length; i++) {

        }
    }
    
    private void RegisterObjectPool(ObjectPoolInitData initData) {
        if (!_objectPools.ContainsKey(initData.Prefab.name)) {
            _objectPools.Add(initData.Prefab.name, new List<ObjectPool>());
        }
        List<ObjectPool> objectPool;
        if(!_objectPools.TryGetValue(initData.Prefab.name, out objectPool)) {
            Debug.LogError($"Unable to retrieve object pool list for Prefab ID: {initData.Prefab.name}");
        }
        for(int i = 0; i < initData.PoolSize; i++) {
            ObjectPool obj = Instantiate(initData.Prefab, transform);
            obj.gameObject.SetActive(false);
            objectPool.Add(obj);
        }
    }
}

[System.Serializable]
public class ObjectPoolInitData {
    public ObjectPool Prefab;
    public int PoolSize;
}
