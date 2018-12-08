using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// What happens when the spell hits
/// </summary>
public abstract class Spell_Effect : ScriptableObject {

    [SerializeField] private int _manaCost; // how much will this effect cost to generate

    public abstract void Trigger();
}
