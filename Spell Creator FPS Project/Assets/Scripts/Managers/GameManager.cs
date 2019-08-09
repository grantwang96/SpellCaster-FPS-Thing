using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameManager : MonoBehaviour {

    public static GameManager Instance;
    protected static GameSave _currentGame;

    protected virtual void Awake() {
        Instance = this;
        _currentGame = SaveLoad.ReadFromDisk();
    }
    
    protected abstract void Initialize();

    protected abstract void SubscribeToInventoryEvents();

    public static void SaveGame() {
        SaveLoad.Save(_currentGame);
    }

    public virtual List<StorableInventoryRune> GetSavedRuneInventory() {
        return new List<StorableInventoryRune>(_currentGame.PlayerRunesInventory);
    }

    public virtual List<StorableSpell> GetSavedSpellsInventory() {
        return new List<StorableSpell>(_currentGame.PlayerSpellsInventory);
    }

    public virtual StorableSpell[] GetSavedLoadout() {
        if(_currentGame.PlayerCurrentLoadout == null) {
            return new StorableSpell[GameplayValues.Magic.PlayerLoadoutMaxSize];
        }
        return (StorableSpell[])_currentGame.PlayerCurrentLoadout.Clone();
    }
}
