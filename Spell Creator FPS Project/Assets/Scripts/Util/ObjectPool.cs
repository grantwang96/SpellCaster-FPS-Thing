using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {

    public static ObjectPool Instance { get; private set; }
    [SerializeField] private ObjectPoolInitData[] _poolPrefabs;

    private Dictionary<string, List<PooledObject>> _availablePooledObjects = new Dictionary<string, List<PooledObject>>();
    private Dictionary<string, List<PooledObject>> _inUsePooledObjects = new Dictionary<string, List<PooledObject>>();

    // HACK LAND WONDERLAND
    public string CheatPrefabId;

	private void Awake() {
        Instance = this;
        InitializePooledObjects();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.U)) {
            CheatSpawnPooledObject();
        }
    }

    private void CheatSpawnPooledObject() {
        PooledObject pooledObject = UsePooledObject(CheatPrefabId);
        pooledObject.ActivatePooledObject();
        RecoveryOrb orb = pooledObject as RecoveryOrb;
        if(orb != null) {
            orb.Initialize(RecoveryOrbType.Mana);
        }
    }

    private void InitializePooledObjects() {
        for (int i = 0; i < _poolPrefabs.Length; i++) {
            RegisterObjectPool(_poolPrefabs[i]);
        }
    }

    // THIS FUNCTION MIGHT NEED TO BE HANDLED IN A LOADING COROUTINE TO REDUCE STUTTERS. GETCOMPONENT IS A THICC BOI
    private void RegisterObjectPool(ObjectPoolInitData initData) {
        PooledObject prefab = initData.Prefab.GetComponent<PooledObject>();
        if(prefab == null) {
            Debug.LogError("WTF Bro?");
            return;
        }
        if (!_availablePooledObjects.ContainsKey(prefab.PrefabId)) {
            _availablePooledObjects.Add(prefab.PrefabId, new List<PooledObject>());
            _inUsePooledObjects.Add(prefab.PrefabId, new List<PooledObject>());
        }
        List<PooledObject> objectPool;
        if(!_availablePooledObjects.TryGetValue(prefab.PrefabId, out objectPool)) {
            Debug.LogError($"Unable to retrieve object pool list for Prefab ID: {prefab.PrefabId}");
            return;
        }
        for(int i = 0; i < initData.PoolSize; i++) {
            GameObject clone = Instantiate(initData.Prefab, transform);
            PooledObject obj = clone.GetComponent<PooledObject>();
            clone.gameObject.SetActive(false);
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
            // don't give anything
            return null;
        }
        // move the object from available to in use
        PooledObject obj = pool[0];
        pool.RemoveAt(0);
        _inUsePooledObjects[id].Add(obj);
        return obj;
    }

    public void ReturnUsedPooledObject(PooledObject obj) {
        // check to see if object id exists
        if (!_inUsePooledObjects.ContainsKey(obj.PrefabId)) {
            Debug.LogError($"Trying to return pooled object with unknown ID: {obj.PrefabId}!");
            return;
        }
        if (!_inUsePooledObjects[obj.PrefabId].Contains(obj)) {
            Debug.LogError($"Object {obj.PrefabId} not found in In-Use list of ID: {obj.PrefabId}!");
            return;
        }
        _inUsePooledObjects[obj.PrefabId].Remove(obj);
        if (!_availablePooledObjects.ContainsKey(obj.PrefabId)) {
            Debug.LogError($"Unable to find pooled object list with ID: {obj.PrefabId}");
            return;
        }
        // move object from one list to another
        _availablePooledObjects[obj.PrefabId].Add(obj);
        _inUsePooledObjects[obj.PrefabId].Remove(obj);
    }
}

[System.Serializable]
public class ObjectPoolInitData {
    public GameObject Prefab;
    public int PoolSize;
}
