using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RoomBlueprint/base")]
/// <summary>
/// Parent class for how rooms may be built
/// </summary>
public class RoomBlueprint : ScriptableObject {

    [Range(1, 4)][SerializeField] protected int _preferredEntranceCount; // preferred number of entrances out of the room(excluding the initial way in)
    public int EntranceCount { get { return _preferredEntranceCount; } }
    [SerializeField] protected int _pillarCount; // odd number SHOULD result in even spread with one pillar in center
    public int PillarCount { get { return _pillarCount; } }

    [SerializeField] protected int _minimumWidth;
    public int MinimumWidth { get { return _minimumWidth; } }
    [SerializeField] protected int _minimumLength;
    public int MinimumLength { get { return _minimumLength; } }
    [SerializeField] protected int _minimumHeight;
    public int MinimumHeight { get { return _minimumHeight; } }

    [SerializeField] protected int _preferredWidth; // x
    public int PreferredWidth { get { return _preferredWidth; } }
    [SerializeField] protected int _preferredLength; // z
    public int PreferredLength { get { return _preferredHeight; } }
    [SerializeField] protected int _preferredHeight; // y
    public int PreferredHeight { get { return _preferredHeight; } }

    public virtual TileData.MapPieceType GetBaseTileTypeAtPosition(IntVector3 dimensionsMin, IntVector3 dimensionsMax, IntVector3 tilePosition) {
        TileData.MapPieceType pieceType = TileData.MapPieceType.NONE;
        if(tilePosition.y == dimensionsMin.y) {
            pieceType = TileData.MapPieceType.FLOOR;
        } else if(tilePosition.y == dimensionsMax.y) {
            pieceType = TileData.MapPieceType.CEILING;
        }
        if (tilePosition.x == dimensionsMin.x || tilePosition.x == dimensionsMax.x ||
           tilePosition.z == dimensionsMin.z || tilePosition.z == dimensionsMax.z) {

            pieceType = pieceType | TileData.MapPieceType.WALL;
        }
        return pieceType;
    }

    public virtual TileData.MapPieceType TryEstablishDoor(TileData.MapPieceType piece, out int entrancesRemaining, int currentEntrances, float chance) {
        entrancesRemaining = currentEntrances;
        if (Random.value < chance) {
            piece &= ~TileData.MapPieceType.WALL;
            piece |= TileData.MapPieceType.DOOR;
            entrancesRemaining--;
        }
        return piece;
    }
}
