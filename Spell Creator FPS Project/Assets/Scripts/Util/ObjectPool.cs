using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class ObjectPool : MonoBehaviour {

    private const int HardPoolCap = 1000; // DO NOT ALLOW ANY POOL SIZE TO EXCEED THIS NUMBER

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

    private void Start() {
        GameStateManager.Instance.OnStateEntered += OnGameStateManagerNewStateEntered;
        OnGameStateManagerNewStateEntered();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.U)) {
            // CheatSpawnPooledObject();
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
            AddObjectToPool(_poolPrefabs[i]);
        }
    }

    private void OnGameStateManagerNewStateEntered() {
        GameStateManager.Instance.CurrentState.OnGameStateExit += OnGameStateExit;
    }

    // when the game is transitioning states
    private void OnGameStateExit() {
        ResetAllInUseObjects();
        GameStateManager.Instance.CurrentState.OnGameStateExit -= OnGameStateExit;
    }

    // THIS FUNCTION MIGHT NEED TO BE HANDLED IN A LOADING COROUTINE TO REDUCE STUTTERS. GETCOMPONENT IS A THICC BOI
    private void AddObjectToPool(ObjectPoolInitData initData) {
        PooledObject prefab = initData.Prefab.GetComponent<PooledObject>();
        if(prefab == null) {
            ErrorManager.LogError(nameof(ObjectPool), "Prefab to be added was null!");
            return;
        }
        if (!_availablePooledObjects.ContainsKey(prefab.PrefabId)) {
            _availablePooledObjects.Add(prefab.PrefabId, new List<PooledObject>());
            _inUsePooledObjects.Add(prefab.PrefabId, new List<PooledObject>());
        }
        CloneToPool(prefab.PrefabId, initData.PoolSize, initData.Prefab);
    }

    public void RegisterObjectToPool(string resourcePath, string prefabName, int count) {
        string path = $"{resourcePath}/{prefabName}";
        GameObject resource = Resources.Load<GameObject>(path);
        if(resource == null) {
            ErrorManager.LogError(nameof(ObjectPool), $"Could not find resource object with name {prefabName}");
            return;
        }
        PooledObject pooledObject = resource.GetComponent<PooledObject>();
        if(pooledObject == null) {
            ErrorManager.LogError(nameof(ObjectPool), $"Resource object was not of type {nameof(PooledObject)}");
            return;
        }
        if (!_availablePooledObjects.ContainsKey(pooledObject.PrefabId)) {
            _availablePooledObjects.Add(prefabName, new List<PooledObject>());
            _inUsePooledObjects.Add(prefabName, new List<PooledObject>());
        }
        CloneToPool(pooledObject.PrefabId, count, resource);
    }

    private void CloneToPool(string prefabName, int count, GameObject resource) {
        for (int i = 0; i < count; i++) {
            if(_availablePooledObjects[prefabName].Count >= HardPoolCap) {
                break;
            }
            GameObject clone = Instantiate(resource, transform);
            PooledObject obj = clone.GetComponent<PooledObject>();
            clone.gameObject.SetActive(false);
            clone.name = prefabName;
            _availablePooledObjects[prefabName].Add(obj);
        }
    }

    private void ResetAllInUseObjects() {
        int failNumber = 0;
        foreach (KeyValuePair<string, List<PooledObject>> inUseList in _inUsePooledObjects) {
            failNumber = _inUsePooledObjects[inUseList.Key].Count;
            int tries = 0;
            while (_inUsePooledObjects[inUseList.Key].Count != 0) {
                PooledObject obj = _inUsePooledObjects[inUseList.Key][0];
                obj.DeactivatePooledObject();
                ReturnUsedPooledObject(obj);
                tries++;
                if(tries > failNumber) {
                    Debug.LogError($"[{nameof(ObjectPool)}] Took too many tries to clear in use object list {inUseList.Key}!");
                    break;
                }
            }
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
*/
