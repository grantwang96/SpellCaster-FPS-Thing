using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour {

    // list of all possible floors
    // list of all possible walls
    // list of all possible ceilings
    // list of all possible doors
    // list of all possible pillars

    public static LevelBuilder Instance;
    public const float TileScale = 5f; // this is for translating theoretical coordinates to world coordinates

    [SerializeField] private int mapX;
    [SerializeField] private int mapY;
    [SerializeField] private int mapZ;

    private TileData[][][] _gameMap;
    public IntVector3[] Directions = {
        IntVector3.Forward,
        IntVector3.Back,
        IntVector3.Up,
        IntVector3.Down,
        IntVector3.Left,
        IntVector3.Right,
    };
    
    void Awake() {
        Instance = this;
    }

	// Use this for initialization
	void Start () {
        InitializeGameMap();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void InitializeGameMap() {
        _gameMap = new TileData[mapX][][];
        for(int i = 0; i < mapX; i++) {
            _gameMap[i] = new TileData[mapY][];
            for(int j = 0; j < mapY; j++) {
                _gameMap[i][j] = new TileData[mapZ];
                for(int k = 0; k < mapZ; k++) {
                    _gameMap[i][j][k] = new TileData(i, j, k, TileData.MapPieceType.NONE, 0);
                }
            }
        }
    }

    // TODO
    private IEnumerator BuildLevelRoutine() {
        yield return null;
    }

    // TODO
    private List<TileData> GetSpecificArea(IntVector3 startPos, TileData.MapPieceType mapPieceType, int maxDistance) {
        List<TileData> tileDatas = new List<TileData>();
        Queue<MapVertex> vertices = new Queue<MapVertex>();
        MapVertex current = new MapVertex(startPos);
        current.Distance = 0;
        vertices.Enqueue(current);
        while(vertices.Count != 0) {
            current = vertices.Dequeue();
            int distance = current.Distance + 1;
            if(distance > maxDistance) { continue; }
            for (int i = 0; i < Directions.Length; i++) {
                IntVector3 neighborPosition = LevelBuildHelper.IntVector3Sum(current.Position, Directions[i]);
                if (!IsWithinMap(neighborPosition)) {
                    continue;
                }
                TileData neighbor = GetTileData(neighborPosition.x, neighborPosition.y, neighborPosition.z);
                if(neighbor.TileType == mapPieceType) {
                    if (!tileDatas.Contains(neighbor)) { tileDatas.Add(neighbor); }
                    MapVertex next = new MapVertex(neighborPosition);
                    next.Distance = distance;
                    next.Previous = current;
                    vertices.Enqueue(next);
                }
            }
        }
        return tileDatas;
    }

    public bool IsWithinMap(IntVector3 position) {
        return (position.x >= 0 && position.x < _gameMap.Length &&
            position.y >= 0 && position.y < _gameMap[0].Length &&
            position.z >= 0 && position.z < _gameMap[0][0].Length);
    }

    public TileData GetTileData(int x, int y, int z) {
        return _gameMap[x][y][z];
    }

    private class MapVertex {

        public MapVertex() { }

        public MapVertex(IntVector3 position) {
            Position = position;
        }

        public MapVertex(int x, int y, int z) {
            Position = new IntVector3(x, y, z);
        }

        public IntVector3 Position; // coordinate in graph of TileData
        public int Distance; // distance away from start of search
        public MapVertex Previous;
    }

    /// <summary>
    /// Establishes a room in the game map. Accepts offsets from entrance to determine room dimensions
    /// </summary>
    /// <param name="offsetMin"></param>
    /// <param name="offsetMax"></param>
    /// <param name="position"></param>
    private void BuildRoom(IntVector3 offsetMin, IntVector3 offsetMax, IntVector3 position) {
        IntVector3 dimensionsMin = new IntVector3();
        IntVector3 dimensionsMax = new IntVector3();
        dimensionsMin.x = position.x - offsetMin.x;
        dimensionsMax.x= position.x + offsetMax.x;
        dimensionsMin.y = position.y - offsetMin.y;
        dimensionsMax.y = position.y + offsetMax.y;
        dimensionsMin.z = position.z - offsetMin.z;
        dimensionsMax.z = position.z + offsetMax.z;
        // TODO: BUILD ROOM BY BLUEPRINT

        for (int i = dimensionsMin.x; i <= dimensionsMax.x; i++) {
            for (int j = dimensionsMin.y; j <= dimensionsMax.y; j++) {
                for (int k = dimensionsMin.z; j <= dimensionsMax.z; k++) {
                    // Instantiate TileData here
                }
            }
        }
    }
}
