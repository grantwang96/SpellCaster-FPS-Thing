using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    // SOME NOTES:

    [SerializeField] private int _xCoord;
    [SerializeField] private int _yCoord;
    [SerializeField] private int _zCoord;
    [SerializeField] private string _roomId;
    [SerializeField] private int _subType;

    [SerializeField] private Transform _tileObject;

    private void Start() {

    }

    /// <summary>
    /// Functions as a constructor of sorts.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public void InitializeData(TileData tileData) {
        _xCoord = tileData.XCoord;
        _yCoord = tileData.YCoord;
        _zCoord = tileData.ZCoord;
        _roomId = tileData.RoomID;
        _subType = tileData.SubType;

        if((tileData.TileType & TileData.MapPieceType.FLOOR) != 0) {
            BuildFloorTile();
        }
        if((tileData.TileType & TileData.MapPieceType.WALL) != 0) {
            BuildWallTile();
        }
        if((tileData.TileType & TileData.MapPieceType.DOOR) != 0) {
            BuildDoorTile();
        }
    }

    private void BuildFloorTile() {
        GameObject newFloor = Instantiate(LevelBuilder.Instance.floorPrefab, transform);
        newFloor.transform.localPosition = Vector3.down * LevelBuilder.Instance.ModifiedTileScale;
    }

    private void BuildWallTile() {
        IntVector3[] directions = new IntVector3[] {
            IntVector3.Forward, IntVector3.Right, IntVector3.Back, IntVector3.Left
        };
        for(int i = 0; i < directions.Length; i++) {
            IntVector3 neighborPosition = new IntVector3(_xCoord, _yCoord, _zCoord);
            neighborPosition = neighborPosition + directions[i];
            IntVector3 oppositeDir = new IntVector3(-directions[i].x, -directions[i].y, -directions[i].z);

            bool isWithinMap = LevelBuilder.Instance.IsWithinMap(neighborPosition);
            if (!isWithinMap) {
                CreateTilePrefab(LevelBuilder.Instance.wallPrefab, oppositeDir);
                continue;
            }

            TileData neighborData = LevelBuilder.Instance.GetTileData(neighborPosition.x, neighborPosition.y, neighborPosition.z);
            if (!neighborData.RoomID.Equals(_roomId)) {
                CreateTilePrefab(LevelBuilder.Instance.wallPrefab, oppositeDir);
                continue;
            }
        }
    }

    private bool IsWallCorner(IntVector3 v1, IntVector3 v2) {
        IntVector3 neighborDir = v1 - v2;
        return neighborDir.x != 0 && neighborDir.z != 0;
    }

    private GameObject CreateTilePrefab(GameObject tilePrefab, IntVector3 dir) {
        Vector3 forward = new Vector3(dir.x, dir.y, dir.z);
        forward = forward.normalized;
        Vector3 localPosition = -forward * LevelBuilder.Instance.ModifiedTileScale;
        GameObject newPrefab = Instantiate(tilePrefab, transform);
        newPrefab.transform.localPosition = localPosition;
        newPrefab.transform.forward = forward;
        return newPrefab;
    }

    private void BuildCeilingTile() {

    }

    private void BuildDoorTile() {
        IntVector3[] directions = new IntVector3[] {
            IntVector3.Forward, IntVector3.Right, IntVector3.Back, IntVector3.Left
        };
        bool spawnedDoor = false;
        for (int i = 0; i < directions.Length; i++) {
            IntVector3 neighborPosition = new IntVector3(_xCoord, _yCoord, _zCoord);
            neighborPosition = neighborPosition + directions[i];
            IntVector3 oppositeDir = new IntVector3(-directions[i].x, -directions[i].y, -directions[i].z);

            bool isWithinMap = LevelBuilder.Instance.IsWithinMap(neighborPosition);
            if (!isWithinMap) { continue; }

            TileData neighborData = LevelBuilder.Instance.GetTileData(neighborPosition.x, neighborPosition.y, neighborPosition.z);
            if (!neighborData.RoomID.Equals(_roomId) && !spawnedDoor) {
                CreateTilePrefab(LevelBuilder.Instance.doorPrefab, oppositeDir);
                spawnedDoor = true;
            } else if (!neighborData.RoomID.Equals(_roomId)) {
                CreateTilePrefab(LevelBuilder.Instance.wallPrefab, oppositeDir);
            }
        }
    }
}
