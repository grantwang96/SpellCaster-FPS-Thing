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
        PlayerInventory.SpellInventory.OnLoadoutDataUpdated += SpellInventory_OnLoadoutDataUpdated;
    }

    protected void SubscribeToPlayerDataEvents() {
        PlayerDataManager.Instance.OnPlayerFlagsUpdated += PlayerDataManager_OnPlayerFlagsUpdated;
        PlayerDataManager.Instance.OnPlayerCountersUpdated += PlayerDataManager_OnPlayerCountersUpdated;
    }

    private void PlayerDataManager_OnPlayerCountersUpdated(IReadOnlyDictionary<string, int> updatedCounters) {
        _currentGame.PlayerDataCounters.Clear();
        foreach(KeyValuePair<string, int> pair in updatedCounters) {
            _currentGame.PlayerDataCounters.Add(pair.Key, pair.Value);
        }
    }

    private void PlayerDataManager_OnPlayerFlagsUpdated(IReadOnlyDictionary<string, bool> updatedFlags) {
        _currentGame.PlayerDataFlags.Clear();
        foreach(KeyValuePair<string, bool> pair in updatedFlags) {
            _currentGame.PlayerDataFlags.Add(pair.Key, pair.Value);
        }
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

    private void SpellInventory_OnLoadoutDataUpdated(StorableSpell[] currentLoadout) {
        _currentGame.PlayerCurrentLoadout = currentLoadout;
    }

    private void OnDestroy() {
        PlayerInventory.RunicInventory.OnRunicInventoryDataUpdated -= RunicInventory_OnRunicInventoryDataUpdated;
        PlayerInventory.SpellInventory.OnSpellInventoryDataUpdated -= SpellInventory_OnSpellInventoryDataUpdated;
        PlayerInventory.SpellInventory.OnLoadoutDataUpdated -= SpellInventory_OnLoadoutDataUpdated;
    }

    // Use this for initialization
    private void Start () {
        Initialize();
	}
}
