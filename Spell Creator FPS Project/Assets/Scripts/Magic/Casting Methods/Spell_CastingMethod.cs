using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the base class for how spells are cast
/// </summary>
public abstract class Spell_CastingMethod : ScriptableObject, IInventoryStorable, ILootableItem {

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

    [SerializeField] private LootTier _lootTier;
    public LootTier LootTier => _lootTier;

    [SerializeField] protected int manaCost;
    public virtual int ManaCost { get { return manaCost; } }
    [SerializeField] protected float _intervalTime;
    public float IntervalTime { get { return _intervalTime; } }
    [SerializeField] protected float _powerScale;
    public float PowerScale => _powerScale;

    public enum SpellTiming {
        Instant, Continuous, Charge
    }
    [SerializeField] protected SpellTiming[] spellTiming;

    /// <summary>
    /// Called when the spell is first cast
    /// </summary>
    /// <param name="caster"></param>
    public virtual bool OnStartCast(ISpellCaster caster, Spell spell) {
        if (ArrayHelper.Contains(spellTiming, SpellTiming.Instant)) {
            CastSpell(caster, spell);
            return true;
        }
        return false;
    }// returns if spell is successfully cast

    /// <summary>
    /// Called while the spell is held
    /// </summary>
    /// <param name="caster"></param>
    public virtual bool OnHoldCast(ISpellCaster caster, Spell spell) {
        if (ArrayHelper.Contains(spellTiming, SpellTiming.Continuous)) {
            if (Mathf.Approximately(caster.ActiveSpell.holdIntervalTime, 0f)) {
                CastSpell(caster, spell);
                return true;
            }
        }
        return false;
    }// returns if spell is successfully cast

    /// <summary>
    /// Called when the spell is released
    /// </summary>
    /// <param name="caster"></param>
    public virtual bool OnEndCast(ISpellCaster caster, Spell spell) {
        if (ArrayHelper.Contains(spellTiming, SpellTiming.Charge)) {
            CastSpell(caster, spell);
            return true;
        }
        return false;
    } // returns if spell is successfully cast

    protected abstract void CastSpell(ISpellCaster caster, Spell spell);

    protected virtual float GetTotalPowerScale(ActiveSpell activeSpell, Spell spell) {
        float powerScale = spell.PowerScale;
        if (ArrayHelper.Contains(spellTiming, SpellTiming.Charge)) {
            powerScale += activeSpell.holdTime / activeSpell.holdIntervalTime;
        }
        return powerScale;
    }
}
