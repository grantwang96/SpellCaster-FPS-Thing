using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerArsenalView : MonoBehaviour {

    [SerializeField] private PlayerCombat _playerCombat;
    [SerializeField] private UIPlayerSpellSlot _playerSpellSlotPrefab;

    private List<UIPlayerSpellSlot> _spellSlots = new List<UIPlayerSpellSlot>();

	// Use this for initialization
	void Start () {
        _playerCombat.OnSpellsInventoryUpdated += OnHeldSpellsUpdated;
        _playerCombat.OnSelectedSpellUpdated += OnSelectedSpellUpdated;

        for(int i = 0; i < GameplayValues.Magic.PlayerLoadoutMaxSize; i++) {
            UIPlayerSpellSlot spellSlot = Instantiate(_playerSpellSlotPrefab, transform);
            spellSlot.gameObject.SetActive(i < _playerCombat.SpellsList.Count);
            _spellSlots.Add(spellSlot);
        }
	}

    private void OnDestroy() {
        _playerCombat.OnSpellsInventoryUpdated -= OnHeldSpellsUpdated;
        _playerCombat.OnSelectedSpellUpdated -= OnSelectedSpellUpdated;
    }

    private void OnHeldSpellsUpdated(List<SpellSlotInfo> slotInfos) {
        for(int i = 0; i < GameplayValues.Magic.PlayerLoadoutMaxSize; i++) {
            _spellSlots[i].gameObject.SetActive(true);
            if(i > slotInfos.Count - 1) {
                _spellSlots[i].gameObject.SetActive(false);
                continue;
            }
            _spellSlots[i].SetSpellSlotInfo(slotInfos[i]);
            _spellSlots[i].SetSelected(i == _playerCombat.SelectedSpellIndex);
        }
    }

    private void OnSelectedSpellUpdated(int slot) {
        for(int i = 0; i < _spellSlots.Count; i++) {
            _spellSlots[i].SetSelected(i == _playerCombat.SelectedSpellIndex);
        }
    }
}

// information for spell slots to handle
public class SpellSlotInfo {
    public readonly string SpellName;
    public readonly Sprite CastingMethodIcons;
    public readonly Sprite[] EffectsIcons;
    public readonly Sprite[] ModifiersIcons;

    public SpellSlotInfo(string spellName, Sprite castingMethod, Sprite[] effects, Sprite[] modifiers) {
        SpellName = spellName;
        CastingMethodIcons = castingMethod;
        EffectsIcons = effects;
        ModifiersIcons = modifiers;
    }
}
