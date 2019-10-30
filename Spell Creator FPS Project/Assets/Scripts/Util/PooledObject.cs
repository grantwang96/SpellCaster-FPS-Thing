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
