using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// this sub panel should not be transitioned into
public class UISpellDescriptionView : UISubPanel {

    public string ContainedSpellId { get; private set; }
    private Spell _containedSpell;

    [SerializeField] private GameObject _contents;

    [SerializeField] private Text _spellName;

    [SerializeField] private Text _castingMethodText;
    [SerializeField] private List<Text> _spellEffectList = new List<Text>();
    [SerializeField] private List<Text> _spellModifierList = new List<Text>();
    
    [SerializeField] private Text _manaCost;

    public override void Initialize(UIPanelInitData initData) {
        base.Initialize(initData);

        SpellDescriptionViewInitData spellDescInit = initData as SpellDescriptionViewInitData;
        if(spellDescInit == null) {
            ShowContents(false);
            return;
        }
        UpdateContainedSpell(spellDescInit.ContainedSpellInstanceId);
    }

    public void UpdateContainedSpell(string spellInstanceId) {
        ContainedSpellId = spellInstanceId;
        StorableSpell storableSpell = GameManager.GameManagerInstance.CurrentSpellInventory.GetSpellByInstanceId(spellInstanceId);
        if(storableSpell == null) {
            // turn this off
            ShowContents(false);
            return;
        }
        ShowContents(true);
        _containedSpell = storableSpell.ConvertToSpell();
        UpdateDisplayInfo();
    }

    private void UpdateDisplayInfo() {
        _castingMethodText.text = _containedSpell.CastingMethod.Name;
        for(int i = 0; i < _spellEffectList.Count; i++) {
            if(i >= _containedSpell.Effects.Length || _containedSpell.Effects[i] == null) {
                _spellEffectList[i].text = "";
                _spellEffectList[i].gameObject.SetActive(false);
                continue;
            }
            _spellEffectList[i].gameObject.SetActive(true);
            _spellEffectList[i].text = _containedSpell.Effects[i].Name;
        }
        for(int i = 0; i < _spellModifierList.Count; i++) {
            if(i >= _containedSpell.SpellModifiers.Length || _containedSpell.SpellModifiers[i] == null) {
                _spellModifierList[i].text = "";
                _spellModifierList[i].gameObject.SetActive(false);
                continue;
            }
            _spellModifierList[i].gameObject.SetActive(true);
            _spellModifierList[i].text = _containedSpell.SpellModifiers[i].Name;
        }
        _spellName.text = $"{_containedSpell.Name}";
        _manaCost.text = $"Mana Cost: {_containedSpell.ManaCost}";
    }

    private void ShowContents(bool show) {
        _contents.SetActive(show);
    }

    public override void SetFocus(bool isFocused, bool hardLocked, IntVector3 dir) {
        base.SetFocus(isFocused, hardLocked, dir);
    }
}

public class SpellDescriptionViewInitData : UIPanelInitData {
    public readonly string ContainedSpellInstanceId;

    public SpellDescriptionViewInitData(string spellInstanceId) {
        ContainedSpellInstanceId = spellInstanceId;
    }
}
