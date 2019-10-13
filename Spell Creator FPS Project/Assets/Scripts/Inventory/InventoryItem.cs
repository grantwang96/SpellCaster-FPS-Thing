using UnityEngine;

[System.Serializable]
public class StorableInventoryRune {
    public string Id { get; protected set; }
    public int Count { get; protected set; }

    public StorableInventoryRune(string id, int count) {
        Id = id;
        Count = count;
    }
}

public enum InventoryItemType {
    INVALID,
    CASTINGMETHOD,
    SPELLEFFECT,
    SPELLMODIFIER,
    SPELL,
}

public interface IInventoryStorable {
    InventoryItemType ItemType { get; }
    string Id { get; }
    Sprite SmallIcon { get; }
    Sprite LargeIcon { get; }
    string Name { get; }
    string ShortDescription { get; }
    string LongDescription { get; }
}

public interface ILootableItem {
    LootTier LootTier { get; }
}

public abstract class SpellComponent : ScriptableObject, IInventoryStorable {
    [SerializeField] protected InventoryItemType _itemType;
    public InventoryItemType ItemType => _itemType;
    [SerializeField] protected string _id;
    public string Id => _id;
    [SerializeField] protected Sprite _smallIcon;
    public Sprite SmallIcon => _smallIcon;
    [SerializeField] protected Sprite _largeIcon;
    public Sprite LargeIcon => _largeIcon;
    [SerializeField] protected string _name;
    public string Name => _name;
    [SerializeField] protected string _shortDescription;
    public string ShortDescription => _shortDescription;
    [SerializeField] protected string _longDescription;
    public string LongDescription => _longDescription;
}