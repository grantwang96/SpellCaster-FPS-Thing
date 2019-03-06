using UnityEngine;
/// <summary>
/// Parent class for inventory items(casting methods, effects, modifiers, etc.)
/// </summary>
public abstract class InventoryItem<T> {

    public int Id { get; protected set; }
    public int Count { get; protected set; }
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

public abstract class SpellComponent : ScriptableObject, IInventoryStorable {
    [SerializeField] protected InventoryItemType _itemType;
    public InventoryItemType ItemType => _itemType;
    [SerializeField] protected string _id;
    public string Id => _id;
    [SerializeField] protected Sprite _icon;
    public Sprite Icon => _icon;
}