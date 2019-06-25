using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellModifier : ScriptableObject, IInventoryStorable {

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

    public abstract void SetupSpell(Spell spell);
    public abstract void SetupProjectile(MagicProjectile projectile);
}
