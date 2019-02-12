using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellModifier : ScriptableObject, IInventoryStorable {

    [SerializeField] private string _id;
    public string Id { get { return _id; } }
    [SerializeField] private Sprite _sprite;
    public Sprite Sprite { get { return _sprite; } }

    public abstract void SetupSpell(Spell spell);
    public abstract void SetupProjectile(MagicProjectile projectile);
}
