using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Included in scene. Handles gameplay events that affect inventory and on screen enemies
/// </summary>
public class GameManager_Gameplay : GameManager {

    protected override void Initialize() {
        SubscribeToInventoryEvents();
    }

    protected override void SubscribeToInventoryEvents() {
        PlayerInventory.RunicInventory.OnRunicInventoryDataUpdated += RunicInventory_OnRunicInventoryDataUpdated;
        PlayerInventory.SpellInventory.OnSpellInventoryDataUpdated += SpellInventory_OnSpellInventoryDataUpdated;
    }

    private void SpellInventory_OnSpellInventoryDataUpdated(IReadOnlyList<StorableSpell> spells) {
        _currentGame.PlayerSpellsInventory.Clear();
        _currentGame.PlayerSpellsInventory.AddRange(spells);
    }

    private void RunicInventory_OnRunicInventoryDataUpdated(IReadOnlyDictionary<string, int> updatedInventory) {
        _currentGame.PlayerRunesInventory.Clear();
        foreach(KeyValuePair<string, int> pair in updatedInventory) {
            StorableInventoryRune newRune = new StorableInventoryRune(pair.Key, pair.Value);
            _currentGame.PlayerRunesInventory.Add(newRune);
        }
    }

    private void OnDestroy() {
        PlayerInventory.RunicInventory.OnRunicInventoryDataUpdated -= RunicInventory_OnRunicInventoryDataUpdated;
        PlayerInventory.SpellInventory.OnSpellInventoryDataUpdated -= SpellInventory_OnSpellInventoryDataUpdated;
    }

    // Use this for initialization
    private void Start () {
        Initialize();
	}
}
