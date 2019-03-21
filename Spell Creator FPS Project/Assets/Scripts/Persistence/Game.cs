using System.Collections.Generic;

[System.Serializable]
public class Game {
    public List<InventoryRune> PlayerRunesInventory = new List<InventoryRune>();
    public List<StorableSpell> PlayerSpellsInventory = new List<StorableSpell>();
}
