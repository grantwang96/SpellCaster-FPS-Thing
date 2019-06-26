using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpellViewCell : UIViewCell, IPointerClickHandler, IPointerEnterHandler {

    [SerializeField] private string _spellInstanceId;
    [SerializeField] private string _spellName;
    [SerializeField] private StorableSpell _spell;

    [SerializeField] private RectTransform _content;
    [SerializeField] private Text _spellNameText;

    public override void Highlight() {
        _content.localScale = Vector3.one * 1.25f;
    }

    public override void Unhighlight() {
        _content.localScale = Vector3.one;
    }

    public override void InteractableSelect() {
        OnSelect();
    }

    public override IUIInteractableData ExtractData() {
        SpellViewCellData data = new SpellViewCellData(xCoord, yCoord);
        data.Name = _spellName;
        data.SetValue(_spellInstanceId);
        return data;
    }

    public void OnPointerClick(PointerEventData eventData) {
        PointerClick();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        PointerEnter();
    }

    public override void Initialize(int x, int y) {
        SpellViewCellData initData = new SpellViewCellData(x, y);
        initData.Name = "";
        initData.Id = "";
        SetValue(initData);
    }

    public override void SetValue(IUIInteractableData data) {
        xCoord = data.X;
        yCoord = data.Y;
        if(data == null) {
            Debug.LogError($"[{nameof(SpellViewCell)}] Initialization data received was null!");
            return;
        }
        SpellViewCellData initData = data as SpellViewCellData;
        if(initData == null) {
            Debug.LogError($"[{nameof(SpellViewCell)}] Initialization data received was not of type {nameof(SpellViewCellData)}!");
            return;
        }
        if(initData.StoredSpell == null) {
            _spellInstanceId = string.Empty;
            _spellName = "Empty Spell Slot";
            UpdateUIValues();
            return;
        }
        _spellInstanceId = initData.StoredSpell.InstanceId;
        _spellName = initData.StoredSpell.Name;
        gameObject.SetActive(true);
        UpdateUIValues();
    }

    private void UpdateUIValues() {
        _spellNameText.text = _spellName;
    }
}

public class SpellViewCellData : ViewCellData{
    
    public StorableSpell StoredSpell { get; private set; }

    public SpellViewCellData(int x, int y) : base(x,y) {
        
    }

    public void SetValue(StorableSpell spell) {
        Id = spell.InstanceId;
        StoredSpell = spell;
    }

    public void SetValue(string instanceId) {
        Id = instanceId;
        StoredSpell = PlayerInventory.SpellInventory.GetSpellByInstanceId(instanceId);
    }
}
