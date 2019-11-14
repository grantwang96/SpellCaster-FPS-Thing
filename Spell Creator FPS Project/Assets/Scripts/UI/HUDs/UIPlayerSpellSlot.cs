using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerSpellSlot : MonoBehaviour {

    [SerializeField] private Text _spellName;
    [SerializeField] private Image _background;
    

    public void SetSpellSlotInfo(SpellSlotInfo info) {
        if(info == null) {
            // set to empty slot
            _spellName.text = "";
            return;
        }
        _spellName.text = info.SpellName;
    }
}
