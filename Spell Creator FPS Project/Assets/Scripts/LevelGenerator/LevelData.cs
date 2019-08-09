using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "LevelData/Base Level Data")]
public class LevelData : ScriptableObject {

    #region LEVEL AESTHETIC
    // list of wall meshes
    // list of floor meshes
    // list of ceiling meshes
    #endregion

    [SerializeField] protected int _mapSizeX;
    public int MapSizeX { get { return _mapSizeX; } }
    [SerializeField] protected int _mapSizeY;
    public int MapSizeY { get { return _mapSizeY; } }
    [SerializeField] protected int _mapSizeZ;
    public int MapSizeZ { get { return _mapSizeZ; } }

    [SerializeField] private List<RoomPoolObject> _roomPool = new List<RoomPoolObject>();
    public IReadOnlyList<RoomPoolObject> RoomPool => _roomPool;
}

[System.Serializable]
public class RoomPoolObject {
    [SerializeField] public RoomBlueprint RoomBlueprint;
    [SerializeField] public int MinCount;
    [SerializeField] public int MaxCount;
    [SerializeField] public int Priority; // lower number means higher priority in list
}