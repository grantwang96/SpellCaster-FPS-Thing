using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains information about the current tile
/// </summary>
/// 
public class TileData {
    
    public enum MapPieceType {
        NONE = 0,
        FLOOR = 1,
        WALL = 2,
        CEILING = 4,
        DOOR = 8,
        PILLAR = 16
    }
    public MapPieceType TileType { get; private set; }

    public int XCoord { get; private set; } // this should be base 1. Have Tile class translate to proper scale
    public int YCoord { get; private set; }
    public int ZCoord { get; private set; }
    public string RoomID { get; private set; }

    public int SubType { get; private set; } // Used to find the exact blueprint registered in appropriate list in LevelBuilder

    public TileData(int x, int y, int z, MapPieceType newType, int subType) {
        XCoord = x;
        YCoord = y;
        ZCoord = z;
        TileType = newType;
        SubType = subType;
    }

    public TileData(int x, int y, int z, MapPieceType newType, int subType, string roomId) {
        XCoord = x;
        YCoord = y;
        ZCoord = z;
        TileType = newType;
        SubType = subType;
        RoomID = roomId;
    }
}
