using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellModifier : ScriptableObject, IInventoryStorable {

    [SerializeField] protected InventoryItemType _itemType;
    public InventoryItemType ItemType => _itemType;
    [SerializeField] protected string _id;
    public string Id => _id;
    [SerializeField] protected Sprite _icon;
    public Sprite Icon => _icon;

    public abstract void SetupSpell(Spell spell);
    public abstract void SetupProjectile(MagicProjectile projectile);
}
