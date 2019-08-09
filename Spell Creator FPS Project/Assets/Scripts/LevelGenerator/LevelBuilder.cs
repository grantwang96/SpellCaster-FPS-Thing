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
    public const float TileScaleOffset = .5f; // allows for some spacing between tile pieces(prevents ultra-thin walls)
    public float ModifiedTileScale {
        get {
            return (TileScale / 2f) - TileScaleOffset;
        }
    }

    private int _mapX;
    private int _mapY;
    private int _mapZ;
    [SerializeField] private int _floorCount;

    [SerializeField] private LevelData _levelData;
    private TileData[][][] _gameMap;
    public IntVector3[] Directions = {
        IntVector3.Forward,
        IntVector3.Back,
        IntVector3.Up,
        IntVector3.Down,
        IntVector3.Left,
        IntVector3.Right,
    };

    [SerializeField] private List<RoomBlueprint> _roomBlueprints = new List<RoomBlueprint>();

    public const string RoomID = "ROOM_{0}";
    private bool _processingCoroutine;

    #region HACKED AF!(DEPRECATED)

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

    private List<IntVector3> InitializeGameMap() {
        _mapX = _levelData.MapSizeX;
        _mapY = _levelData.MapSizeY;
        _mapZ = _levelData.MapSizeZ;

        List<IntVector3> availablePoints = new List<IntVector3>();
        _gameMap = new TileData[_mapX][][];
        for(int i = 0; i < _mapX; i++) {
            _gameMap[i] = new TileData[_mapY][];
            for(int j = 0; j < _mapY; j++) {
                _gameMap[i][j] = new TileData[_mapZ];
                for(int k = 0; k < _mapZ; k++) {
                    _gameMap[i][j][k] = new TileData(i, j, k, TileData.MapPieceType.NONE, 0, string.Empty);
                    availablePoints.Add(new IntVector3(i, j, k));
                }
            }
        }
        return availablePoints;
    }

    // TODO
    private IEnumerator BuildLevelRoutine() {
        StartCoroutine(LayoutLevel());
        // IntVector3 offsetMin = new IntVector3(MinimumOffsetXLowerBound, MinimumOffsetYLowerBound, MinimumOffsetZLowerBound);
        // IntVector3 offsetMax = new IntVector3(MaximumOffsetXUpperBound, MaximumOffsetYUpperBound, MaximumOffsetZUpperBound);
        // IntVector3 position = new IntVector3(levelStartX, levelStartY, levelStartZ);

        while (_processingCoroutine) {
            yield return null;
        }
        StartCoroutine(BuildMap());
        while (_processingCoroutine) {
            yield return null;
        }
        Debug.Log("Done!");
    }

    private IEnumerator LayoutLevel() {
        Debug.Log("Laying out map...");
        _processingCoroutine = true;
        List<IntVector3> availablePoints = InitializeGameMap();
        Vector3 previousPoint = Vector3.one * 0.5f;
        LoadRoomPool();
        yield return null;
        for(int i = 0; i < _roomBlueprints.Count; i++) {
            RoomBlueprint blueprint = _roomBlueprints[i];
            Vector3 vector3StartLocation = GetPseudoRandomPoint(previousPoint);
            previousPoint = vector3StartLocation;
            int roomX = Mathf.RoundToInt(_gameMap.Length * vector3StartLocation.x);
            int roomY = Mathf.RoundToInt(_gameMap[0].Length * vector3StartLocation.y);
            int roomZ = Mathf.RoundToInt(_gameMap[0][0].Length * vector3StartLocation.z);

            TryGenerateRoom(roomX, roomY, roomZ, blueprint);
            yield return null;
        }
        _processingCoroutine = false;
    }

    private void LoadRoomPool() {
        _roomBlueprints.Clear();
        for(int i = 0; i < _levelData.RoomPool.Count; i++) {
            RoomPoolObject roomPoolObject = _levelData.RoomPool[i];
            int count = Random.Range(roomPoolObject.MinCount, roomPoolObject.MaxCount);
            for(int j = 0; j < count; j++) {
                _roomBlueprints.Add(roomPoolObject.RoomBlueprint);
            }
        }
    }

    private void PriorityShuffleRooms() {

    }

    private Vector3 GetPseudoRandomPoint(Vector3 previous) {
        bool posX = Random.value < previous.x;
        bool posY = Random.value < previous.y;
        bool posZ = Random.value < previous.z;

        Vector3 newRandomPoint = new Vector3();
        newRandomPoint.x = posX ? Random.Range(previous.x, 1f) : Random.Range(0f, previous.x);
        newRandomPoint.y = posY ? Random.Range(previous.y, 1f) : Random.Range(0f, previous.y);
        newRandomPoint.z = posZ ? Random.Range(previous.z, 1f) : Random.Range(0f, previous.z);
        return newRandomPoint;
    }

    private bool TryGenerateRoom(int x, int y, int z, RoomBlueprint blueprint) {
        // Generate lower and upper bound offsets based on preferred dimensions of room blueprint
        IntVector3 lowerBound = new IntVector3();
        lowerBound.x = -Random.Range(0, blueprint.PreferredWidth / 2);
        lowerBound.y = -Random.Range(0, blueprint.PreferredHeight / 2);
        lowerBound.z = -Random.Range(0, blueprint.PreferredLength / 2);
        IntVector3 upperBound = new IntVector3();
        upperBound.x = blueprint.PreferredWidth + lowerBound.x;
        upperBound.y = blueprint.PreferredHeight + lowerBound.y;
        upperBound.z = blueprint.PreferredLength + lowerBound.z;

        Debug.Log("Lower bound Before: " + new Vector3(lowerBound.x, lowerBound.y, lowerBound.z));
        Debug.Log("Upper bound Before: " + new Vector3(upperBound.x, upperBound.y, upperBound.z));

        // modify offsets if necessary
        bool roomFits = false;
        while (!roomFits) {
            roomFits = true;
            for(int i = lowerBound.x; i <= upperBound.x; i++) {
                // break early if we are out of bounds
                if (!roomFits) {
                    break;
                }
                if(x + i < 0 || x + i >= _gameMap.Length) {
                    upperBound.x = i - 1;
                    roomFits = false;
                    break;
                }
                Debug.Log("GameMap Length: " + _gameMap.Length);
                Debug.Log("i: " + i);
                for (int j = lowerBound.y; j <= upperBound.y; j++) {
                    if (!roomFits) {
                        break;
                    }
                    if (y + j < 0 || y + j >= _gameMap[x + i].Length) {
                        upperBound.y = j - 1;
                        roomFits = false;
                        break;
                    }
                    for (int k = lowerBound.z; k <= upperBound.z; k++) {
                        if (!roomFits) {
                            break;
                        }
                        if (z + k < 0 || z + k >= _gameMap[x + i][y + j].Length) {
                            upperBound.z = k - 1;
                            roomFits = false;
                            break;
                        }
                        TileData tileData = _gameMap[x + i][y + j][z + k];
                        if(tileData.TileType != TileData.MapPieceType.NONE) {
                            roomFits = false;
                            upperBound.x = i - 1;
                            upperBound.y = j - 1;
                            upperBound.z = k - 1;
                            break;
                        }
                    }
                }
            }
        }

        Debug.Log("Lower bound After: " + new Vector3(lowerBound.x, lowerBound.y, lowerBound.z));
        Debug.Log("Upper bound After: " + new Vector3(upperBound.x, upperBound.y, upperBound.z));

        // Validate that those offsets are possible dimensions for the room
        if (ValidateRoomDimensions(lowerBound, upperBound)) {
            LayoutRoom(lowerBound, upperBound, new IntVector3(x, y, z), blueprint);
            return true;
        }

        // Establish the room TileData
        return false;
    }

    private bool ValidateRoomDimensions(IntVector3 lowerBound, IntVector3 upperBound) {
        return lowerBound.x < upperBound.x && lowerBound.y < upperBound.y && lowerBound.z < upperBound.z;
    }

    /// <summary>
    /// Establishes a room in the game map. Accepts offsets from entrance to determine room dimensions
    /// </summary>
    /// <param name="offsetMin"></param>
    /// <param name="offsetMax"></param>
    /// <param name="position"></param>
    private void LayoutRoom(IntVector3 offsetMin, IntVector3 offsetMax, IntVector3 position, RoomBlueprint roomBlueprint) {
        Debug.Log("Laying out room");
        _processingCoroutine = true;
        IntVector3 dimensionsMin = new IntVector3();
        IntVector3 dimensionsMax = new IntVector3();
        dimensionsMin.x = position.x + offsetMin.x;
        dimensionsMax.x = position.x + offsetMax.x;
        dimensionsMin.y = position.y + offsetMin.y;
        dimensionsMax.y = position.y + offsetMax.y;
        dimensionsMin.z = position.z + offsetMin.z;
        dimensionsMax.z = position.z + offsetMax.z;

        int entranceCount = roomBlueprint.EntranceCount;
        int floorPerimeter = 2 * ((dimensionsMax.x - dimensionsMin.x) + (dimensionsMax.z - dimensionsMin.z));

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
                    if (entranceCount > 0 && (piece & TileData.MapPieceType.FLOOR) != 0 && (piece & TileData.MapPieceType.WALL) != 0) {
                        float doorChance = 1f / floorPerimeter;
                        piece = roomBlueprint.TryEstablishDoor(piece, out entranceCount, entranceCount, doorChance);
                        floorPerimeter--;
                    }

                    _gameMap[i][j][k] = new TileData(i, j, k, piece, 0, roomId);
                }
            }
        }
        _processingCoroutine = false;
    }

    private IEnumerator BuildMap() {
        Debug.Log("Building map...");
        _processingCoroutine = true;
        for (int i = 0; i < _gameMap.Length; i++) {
            for (int j = 0; j < _gameMap[i].Length; j++) {
                for (int k = 0; k < _gameMap[i][j].Length; k++) {
                    BuildMapPiece(i, j, k);
                }
            }
            yield return null;
        }
        _processingCoroutine = false;
    }

    private void BuildMapPiece(int x, int y, int z) {
        TileData currentTileData = _gameMap[x][y][z];
        IntVector3 gridPos = new IntVector3(x, y, z);
        Vector3 worldPos = IntVector3Builder.GridToWorldCoordinates(gridPos, TileScale, _mapX, _mapY, _mapZ);

        if (currentTileData.TileType != TileData.MapPieceType.NONE) {
            Tile newTile = Instantiate(_tilePrefab, worldPos, Quaternion.identity);
            newTile.InitializeData(currentTileData);
        }
    }

    private List<IntVector3> GetSpecificAreaByTileType(IntVector3 startPos, TileData.MapPieceType mapPieceType, int maxDistance) {
        List<IntVector3> positions = new List<IntVector3>();
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
                    if (!positions.Contains(neighborPosition)) { positions.Add(neighborPosition); }
                    MapVertex next = new MapVertex(neighborPosition);
                    next.Distance = distance;
                    next.Previous = current;
                    vertices.Enqueue(next);
                }
            }
        }
        return positions;
    }

    private List<IntVector3> GetSpecificAreaByRoomId(IntVector3 startPos, string roomId) {
        List<IntVector3> positions = new List<IntVector3>();
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
                    if (!positions.Contains(neighborPosition)) { positions.Add(neighborPosition); }
                    MapVertex next = new MapVertex(neighborPosition);
                    next.Previous = current;
                    vertices.Enqueue(next);
                }
            }
        }
        return positions;
    }

    private IntVector3 GetLargestRectangularDimensions(List<IntVector3> positions) {
        IntVector3 dimensions = new IntVector3();

        for(int i = 0; i < positions.Count; i++) {

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

    // helper class used for pathfinding
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

    /*
    public static LevelBuilder Instance;
    public const float TileScale = 5f; // this is for translating theoretical coordinates to world coordinates
    public const float TileScaleOffset = .5f; // allows for some spacing between tile pieces(prevents ultra-thin walls)
    public float ModifiedTileScale {
        get {
            return (TileScale / 2f) - TileScaleOffset;
        }
    }

    public Tile _tilePrefab;
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject doorPrefab;

    // dimensions of the whole map
    [SerializeField] private int _mapX;
    [SerializeField] private int _mapY;
    [SerializeField] private int _mapZ;

    private TileData[][][] _gameMap;

    private void InitializeGameMap() {
        _gameMap = new TileData[_mapX][][];
        for(int x = 0; x < _mapX; x++) {
            _gameMap[x] = new TileData[_mapY][];
            for(int y = 0; y < _mapY; y++) {
                _gameMap[x][y] = new TileData[_mapZ];
                for(int z = 0; z < _mapZ; z++) {
                    _gameMap[x][y][z] = new TileData(x, y, z, TileData.MapPieceType.NONE, 0);
                }
            }
        }
    }

    private IEnumerator BuildLevel() {
        yield return null;
    }

    private void EstablishRoom(IntVector3 position, RoomBlueprint blueprint) {
        int roomSizeX = 
    }

    public bool IsWithinMap(IntVector3 position) {
        return position.x >= 0 && position.x < _mapX &&
            position.y >= 0 && position.y < _mapY &&
            position.z >= 0 && position.z < _mapZ;
    }

    public TileData GetTileData(int x, int y, int z) {
        return new TileData(x, y, z, TileData.MapPieceType.FLOOR, 0);
    }

    [System.Serializable]
    private class RoomData {
        [SerializeField] public string _roomPrefabName;
        [SerializeField] public List<RoomBlueprint> _possibleConnectingRooms;
    }
    */
}
