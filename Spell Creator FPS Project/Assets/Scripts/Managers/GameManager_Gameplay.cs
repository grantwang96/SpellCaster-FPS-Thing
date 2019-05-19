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

    private void SpellInventory_OnSpellInventoryDataUpdated() {
        _currentGame.PlayerSpellsInventory.Clear();
        _currentGame.PlayerSpellsInventory.AddRange(PlayerInventory.SpellInventory.RetrieveAllSpells());
    }

    private void RunicInventory_OnRunicInventoryDataUpdated() {
        _currentGame.PlayerRunesInventory.Clear();
        IReadOnlyList<KeyValuePair<string, int>> runicInventory = PlayerInventory.RunicInventory.RetrieveAllItems();
        
        for(int i = 0; i < runicInventory.Count; i++) {
            InventoryRune newRune = new InventoryRune(runicInventory[i].Key, runicInventory[i].Value);
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
