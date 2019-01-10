using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains information about the current tile
/// </summary>
/// 
[System.Serializable]
public class TileData {
    
    public enum MapPieceType {
        NONE,
        FLOOR,
        WALL,
        CEILING,
        DOOR,
        PILLAR
    }
    public MapPieceType TileType { get; private set; }

    public int XCoord { get; private set; } // this should be base 1. Have Tile class translate to proper scale
    public int YCoord { get; private set; }
    public int ZCoord { get; private set; }

    public int SubType { get; private set; } // Used to find the exact blueprint registered in appropriate list in LevelBuilder

    public TileData(int x, int y, int z, MapPieceType newType, int subType) {
        XCoord = x;
        YCoord = y;
        ZCoord = z;
        TileType = newType;
        SubType = subType;
    }
}
