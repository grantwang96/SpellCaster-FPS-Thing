using UnityEngine;

[System.Serializable]
public class InventoryRune {
    public string Id { get; protected set; }
    public int Count { get; protected set; }

    public InventoryRune(string id, int count) {
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
    Sprite Icon { get; }
}

public interface ILootableItem {

}

public abstract class SpellComponent : ScriptableObject, IInventoryStorable {
    [SerializeField] protected InventoryItemType _itemType;
    public InventoryItemType ItemType => _itemType;
    [SerializeField] protected string _id;
    public string Id => _id;
    [SerializeField] protected Sprite _icon;
    public Sprite Icon => _icon;
}