using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// parent class of managers that control the game mode
public abstract class GameManager : MonoBehaviour {

    public static GameManager GameManagerInstance { get; protected set; }

    public IRunicInventory CurrentRunicInventory { get; protected set; }
    public ISpellInventory CurrentSpellInventory { get; protected set; }

    protected virtual void Awake() {
        GameManagerInstance = this;
        SetInventories();
    }

    protected virtual void Start() {
        SubscribeToEvents();
    }

    protected virtual void SubscribeToEvents() {
        PlayerController.Instance.Damageable.OnDeath += OnPlayerDefeated;
        GameStateManager.Instance.OnStateExited += OnCurrentGameStateExited;
    }

    protected virtual void UnsubscribeToEvents() {
        PlayerController.Instance.Damageable.OnDeath -= OnPlayerDefeated;
        GameStateManager.Instance.OnStateExited -= OnCurrentGameStateExited;
    }

    protected virtual void OnPlayerDefeated(bool isDead, Damageable damageable) {

    }

    protected virtual void OnCurrentGameStateExited() {
        UnsubscribeToEvents();
    }

    protected virtual void SetInventories() {
        TempInventory tempInventory = new TempInventory(true);
        CurrentRunicInventory = tempInventory;
        CurrentSpellInventory = tempInventory;
    }

    protected virtual void MovePlayerInventoryToPersistence() {
        if(CurrentRunicInventory != PersistedInventory.RunicInventory) {
            IReadOnlyList<KeyValuePair<string, int>> allRunes = CurrentRunicInventory.RetrieveAllItems();
            for(int i = 0; i < allRunes.Count; i++) {
                PersistedInventory.RunicInventory.AddItem(allRunes[i].Key, allRunes[i].Value);
            }
        }
        if(CurrentSpellInventory != PersistedInventory.SpellInventory) {
            IReadOnlyList<StorableSpell> allSpells = CurrentSpellInventory.StoredSpells;
            for(int i = 0; i < allSpells.Count; i++) {
                PersistedInventory.SpellInventory.AddSpell(allSpells[i]);
            }
        }
    }
}
