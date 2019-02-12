using UnityEngine;
/// <summary>
/// Parent class for inventory items(casting methods, effects, modifiers, etc.)
/// </summary>
public abstract class InventoryItem<T> {

    public int Id { get; protected set; }
    public int Count { get; protected set; }
}

public interface IInventoryStorable {

    string Id { get; }
    Sprite Sprite { get; }
}