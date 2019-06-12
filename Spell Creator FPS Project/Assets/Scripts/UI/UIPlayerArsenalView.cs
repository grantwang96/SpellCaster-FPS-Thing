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

        for(int i = 0; i < _playerCombat.SpellInventoryLimit; i++) {
            UIPlayerSpellSlot spellSlot = Instantiate(_playerSpellSlotPrefab, transform);
            spellSlot.gameObject.SetActive(i < _playerCombat.SpellsList.Count);
            _spellSlots.Add(spellSlot);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDestroy() {
        _playerCombat.OnSpellsInventoryUpdated -= OnHeldSpellsUpdated;
    }

    private void OnHeldSpellsUpdated(List<SpellSlotInfo> slotInfos) {
        for(int i = 0; i < _playerCombat.SpellInventoryLimit; i++) {
            _spellSlots[i].gameObject.SetActive(true);
            if(i > slotInfos.Count - 1) {
                _spellSlots[i].gameObject.SetActive(false);
                continue;
            }
            _spellSlots[i].SetSpellSlotInfo(slotInfos[i]);
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
