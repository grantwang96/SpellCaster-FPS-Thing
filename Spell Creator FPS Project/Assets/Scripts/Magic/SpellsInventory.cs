using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellsInventory : MonoBehaviour {
    
    [SerializeField] private List<Spell> spellsList;
    public List<Spell> SpellsList { get { return spellsList; } }

    public void SetNewSpellsList(List<Spell> newSpellsList) {
        spellsList = newSpellsList;
    }

    public void PickUpSpell(Spell spell) {

    }

    public void DropSpell(Spell spell) {

    }
}
