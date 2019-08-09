using System.Collections.Generic;

[System.Serializable]
public class GameSave {
    public List<StorableInventoryRune> PlayerRunesInventory = new List<StorableInventoryRune>();
    public List<StorableSpell> PlayerSpellsInventory = new List<StorableSpell>();
    public StorableSpell[] PlayerCurrentLoadout = new StorableSpell[GameplayValues.Magic.PlayerLoadoutMaxSize];
    public Dictionary<string, bool> PlayerDataFlags = new Dictionary<string, bool>();
    public Dictionary<string, int> PlayerDataCounters = new Dictionary<string, int>();
}
