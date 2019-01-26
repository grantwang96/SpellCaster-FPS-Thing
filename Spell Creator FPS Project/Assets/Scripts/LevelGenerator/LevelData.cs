using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "LevelData/Base Level Data")]
public class LevelData : ScriptableObject {

    // list of wall meshes
    // list of floor meshes
    // list of ceiling meshes

    [SerializeField] protected int x;
    public int X { get { return x; } }
    [SerializeField] protected int y;
    public int Y { get { return y; } }
    [SerializeField] protected int z;
    public int Z { get { return z; } }

    public List<int> roomBlueprintIndices = new List<int>();
}

[System.Serializable]
public class RoomCount {
    public int RoomBlueprintIndex;
    public int Count;
}