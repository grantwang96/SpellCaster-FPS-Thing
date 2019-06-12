using System.Collections.Generic;
/// <summary>
/// Contains data required to initialize the game when the player loads into a level
/// </summary>
public class GameInitData {
    public readonly List<Spell> StartingSpells;

    public GameInitData(List<Spell> startingSpells) {
        StartingSpells = startingSpells;
    }
}