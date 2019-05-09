using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {

    public static ObjectPool Instance { get; private set; }
    [SerializeField] private ObjectPoolInitData[] _poolPrefabs;

    private Dictionary<string, List<PooledObject>> _availablePooledObjects = new Dictionary<string, List<PooledObject>>();
    private Dictionary<string, List<PooledObject>> _inUsePooledObjects = new Dictionary<string, List<PooledObject>>();

	private void Awake() {
        Instance = this;
        InitializePooledObjects();
    }

    private void InitializePooledObjects() {
        for (int i = 0; i < _poolPrefabs.Length; i++) {
            RegisterObjectPool(_poolPrefabs[i]);
        }
    }
    
    private void RegisterObjectPool(ObjectPoolInitData initData) {
        if (!_availablePooledObjects.ContainsKey(initData.Prefab.name)) {
            _availablePooledObjects.Add(initData.Prefab.name, new List<PooledObject>());
            _inUsePooledObjects.Add(initData.Prefab.name, new List<PooledObject>());
        }
        List<PooledObject> objectPool;
        if(!_availablePooledObjects.TryGetValue(initData.Prefab.name, out objectPool)) {
            Debug.LogError($"Unable to retrieve object pool list for Prefab ID: {initData.Prefab.name}");
        }
        for(int i = 0; i < initData.PoolSize; i++) {
            PooledObject obj = Instantiate(initData.Prefab, transform);
            obj.gameObject.SetActive(false);
            objectPool.Add(obj);
        }
    }

    public PooledObject UsePooledObject(string id) {
        // make sure requested object exists within pool
        if (!_availablePooledObjects.ContainsKey(id)) {
            Debug.LogError($"Requested pooled object ID: {id} not registered!");
            return null;
        }
        List<PooledObject> pool = _availablePooledObjects[id];
        // if all objects are in use at the moment
        if(pool.Count == 0) {
            // if the in use list is also somehow empty(means you left the init data at 0, dingus)
            if(_inUsePooledObjects[id].Count == 0) {
                Debug.LogError($"In Use Pool for ID {id} is also empty, dingus!");
                return null;
            }
            // TOOD: DEACTIVATE AND RE INITIALIZE OBJECT
            _inUsePooledObjects[id][0].DeactivatePooledObject();
            return _inUsePooledObjects[id][0];
        }
        // move the object from available to in use
        PooledObject obj = pool[0];
        pool.RemoveAt(0);
        obj.SetPooledObjectPrefabId(id);
        _inUsePooledObjects[id].Add(obj);
        return obj;
    }

    public void ReturnUsedPooledObject(string id, PooledObject obj) {
        // check to see if object id exists
        if (!_inUsePooledObjects.ContainsKey(id)) {
            Debug.LogError($"Trying to return pooled object with unknown ID: {id}!");
            return;
        }
        if (!_inUsePooledObjects[id].Contains(obj)) {
            Debug.LogError($"Object {obj.name} not found in In-Use list of ID: {id}!");
            return;
        }
        _inUsePooledObjects[id].Remove(obj);
        if (!_availablePooledObjects.ContainsKey(id)) {
            Debug.LogError($"Unable to find pooled object list with ID: {id}");
            return;
        }
        // move object from one list to another
        _availablePooledObjects[id].Add(obj);
        _inUsePooledObjects[id].Remove(obj);
    }
}

[System.Serializable]
public class ObjectPoolInitData {
    public PooledObject Prefab;
    public int PoolSize;
}
