using UnityEngine;

/// <summary>
/// Objects that may appear many times in world space
/// </summary>
public interface PooledObject {
    
    string PrefabId { get; }
    string UniqueId { get; }
    bool InUse { get; }

    void DeactivatePooledObject();
    void ActivatePooledObject(string uniqueId = "");
}

[System.Serializable]
public class PooledObjectEntry {
    [SerializeField] private string _id;
    public string Id => _id;
    [SerializeField] private int _count;
    public int Count => _count;
}