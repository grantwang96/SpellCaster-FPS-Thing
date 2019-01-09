using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parent class for how rooms may be built
/// </summary>
public class RoomBlueprint : ScriptableObject {

    [Range(1, 4)][SerializeField] protected int _entranceCount;
    public int EntranceCount { get { return _entranceCount; } }
    [SerializeField] protected int _pillarCount; // odd number will result in even spread with one pillar in center
    public int PillarCount { get { return _pillarCount; } }

    public virtual TileData.MapPieceType GetTileTypeAtPosition(IntVector3 dimensionsMin, IntVector3 dimensionsMax, IntVector3 tilePosition) {
        // establish corners of the room
        TileData.MapPieceType pieceType = TileData.MapPieceType.NONE;
        if(tilePosition.y == dimensionsMin.y) {
            pieceType = TileData.MapPieceType.FLOOR;
        }
        if (tilePosition.x == dimensionsMin.x || tilePosition.x == dimensionsMax.x ||
           tilePosition.z == dimensionsMin.z || tilePosition.z == dimensionsMax.z) {
            pieceType = pieceType | TileData.MapPieceType.WALL;
        }
        return pieceType;
    }
}
