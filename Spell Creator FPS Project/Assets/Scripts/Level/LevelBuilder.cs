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
    public const float TileScaleOffset = .5f;
    public float ModifiedTileScale {
        get {
            return (TileScale / 2f) - TileScaleOffset;
        }
    }

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
    [SerializeField] private RoomBlueprint[] _roomBlueprints;

    public const string RoomID = "ROOM_{0}";

    #region HACKED AF!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    private bool processingCoroutine;

    public int MinimumOffsetXLowerBound;
    public int MaximumOffsetXUpperBound;
    public int MinimumOffsetYLowerBound;
    public int MaximumOffsetYUpperBound;
    public int MinimumOffsetZLowerBound;
    public int MaximumOffsetZUpperBound;

    private int RoomNumber = 0;

    public int levelStartX;
    public int levelStartY;
    public int levelStartZ;
    #endregion

    public Tile _tilePrefab;
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject doorPrefab;

    void Awake() {
        Instance = this;
    }

	// Use this for initialization
	void Start () {
        StartCoroutine(BuildLevelRoutine());
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
                    _gameMap[i][j][k] = new TileData(i, j, k, TileData.MapPieceType.NONE, 0, string.Empty);
                }
            }
        }
    }

    // TODO
    private IEnumerator BuildLevelRoutine() {
        InitializeGameMap();
        IntVector3 offsetMin = new IntVector3(MinimumOffsetXLowerBound, MinimumOffsetYLowerBound, MinimumOffsetZLowerBound);
        IntVector3 offsetMax = new IntVector3(MaximumOffsetXUpperBound, MaximumOffsetYUpperBound, MaximumOffsetZUpperBound);
        IntVector3 position = new IntVector3(levelStartX, levelStartY, levelStartZ);
        StartCoroutine(LayoutRoom(offsetMin, offsetMax, position));
        while (processingCoroutine) {
            yield return null;
        }
        StartCoroutine(BuildMap());
        while (processingCoroutine) {
            yield return null;
        }
        Debug.Log("Done!");
    }

    // TODO
    private List<TileData> GetSpecificAreaByTileType(IntVector3 startPos, TileData.MapPieceType mapPieceType, int maxDistance) {
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
                IntVector3 neighborPosition = current.Position + Directions[i];
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

    private List<TileData> GetSpecificAreaByRoomId(IntVector3 startPos, string roomId) {
        List<TileData> tileDatas = new List<TileData>();
        Queue<MapVertex> vertices = new Queue<MapVertex>();
        MapVertex current = new MapVertex(startPos);
        current.Distance = 0;
        vertices.Enqueue(current);
        while (vertices.Count != 0) {
            current = vertices.Dequeue();
            for (int i = 0; i < Directions.Length; i++) {
                IntVector3 neighborPosition = current.Position + Directions[i];
                if (!IsWithinMap(neighborPosition)) {
                    continue;
                }
                TileData neighbor = GetTileData(neighborPosition.x, neighborPosition.y, neighborPosition.z);
                if (neighbor.RoomID.Equals(roomId)) {
                    if (!tileDatas.Contains(neighbor)) { tileDatas.Add(neighbor); }
                    MapVertex next = new MapVertex(neighborPosition);
                    next.Previous = current;
                    vertices.Enqueue(next);
                }
            }
        }
        return tileDatas;
    }

    private IntVector3 GetLargestRectangularDimensions(List<TileData> tileDatas) {
        IntVector3 dimensions = new IntVector3();
        IntVector3 minimum = new IntVector3();
        IntVector3 maximum = new IntVector3();

        for(int i = 0; i < tileDatas.Count; i++) {

        }


        return dimensions;
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
    private IEnumerator LayoutRoom(IntVector3 offsetMin, IntVector3 offsetMax, IntVector3 position) {
        Debug.Log("Laying out room");
        processingCoroutine = true;
        IntVector3 dimensionsMin = new IntVector3();
        IntVector3 dimensionsMax = new IntVector3();
        dimensionsMin.x = position.x - offsetMin.x;
        dimensionsMax.x= position.x + offsetMax.x;
        dimensionsMin.y = position.y - offsetMin.y;
        dimensionsMax.y = position.y + offsetMax.y;
        dimensionsMin.z = position.z - offsetMin.z;
        dimensionsMax.z = position.z + offsetMax.z;
        // TODO: BUILD ROOM BY BLUEPRINT

        // HACK
        RoomBlueprint roomBlueprint = _roomBlueprints[0];
        int entranceCount = roomBlueprint.EntranceCount;
        int floorPerimeter = 2*((dimensionsMax.x - dimensionsMin.x) + (dimensionsMax.z - dimensionsMin.z));

        string roomId = string.Format(RoomID, RoomNumber.ToString());
        for (int i = dimensionsMin.x; i <= dimensionsMax.x; i++) {
            for (int j = dimensionsMin.y; j <= dimensionsMax.y; j++) {
                for (int k = dimensionsMin.z; k <= dimensionsMax.z; k++) {
                    IntVector3 tilePosition = new IntVector3(i, j, k);
                    TileData.MapPieceType piece = roomBlueprint.GetBaseTileTypeAtPosition(dimensionsMin, dimensionsMax, tilePosition);

                    if (IntVector3Builder.IntVector3Equals(tilePosition, position)) {
                        piece |= TileData.MapPieceType.FLOOR;
                        piece &= ~TileData.MapPieceType.WALL;
                        piece |= TileData.MapPieceType.DOOR;
                        entranceCount--;
                    }
                    if(entranceCount > 0 && (piece & TileData.MapPieceType.FLOOR) != 0 && (piece & TileData.MapPieceType.WALL) != 0) {
                        float doorChance = 1f / floorPerimeter;
                        piece = roomBlueprint.TryEstablishDoor(piece, out entranceCount, entranceCount, doorChance);
                        floorPerimeter--;
                    }

                    _gameMap[i][j][k] = new TileData(i, j, k, piece, 0, roomId);
                    yield return null;
                }
            }
        }
        processingCoroutine = false;
    }

    private void EstablishRoomWithinArea(List<TileData> tileDatas) {

    }

    private IEnumerator BuildMap() {
        Debug.Log("Building map...");
        processingCoroutine = true;
        for(int i = 0; i < _gameMap.Length; i++) {
            for(int j = 0; j < _gameMap[i].Length; j++) {
                for(int k = 0; k < _gameMap[i][j].Length; k++) {
                    BuildMapPiece(i, j, k);
                }
            }
            yield return null;
        }
        processingCoroutine = false;
    }

    private void BuildMapPiece(int x, int y, int z) {
        TileData currentTileData = _gameMap[x][y][z];
        IntVector3 gridPos = new IntVector3(x, y, z);
        Vector3 worldPos = IntVector3Builder.GridToWorldCoordinates(gridPos, TileScale, mapX, mapY, mapZ);

        if(currentTileData.TileType != TileData.MapPieceType.NONE) {
            Tile newTile = Instantiate(_tilePrefab, worldPos, Quaternion.identity);
            newTile.InitializeData(currentTileData);
        }
    }
}
