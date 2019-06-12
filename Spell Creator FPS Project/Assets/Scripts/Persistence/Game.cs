using System.Collections.Generic;

[System.Serializable]
public class Game {
    public List<StorableInventoryRune> PlayerRunesInventory = new List<StorableInventoryRune>();
    public List<StorableSpell> PlayerSpellsInventory = new List<StorableSpell>();
}
