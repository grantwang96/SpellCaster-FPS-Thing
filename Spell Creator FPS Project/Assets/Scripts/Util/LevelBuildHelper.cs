using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This helps with IntVector3 functionality
/// SHOULD NOT INCLUDE ANYTHING FROM LEVELBUILDER
/// </summary>
public static class IntVector3Builder {

    public static Vector3 GridToWorldCoordinates(IntVector3 gridCoord, float tileScale, int mapX, int mapY, int mapZ) {
        float halfX = mapX / 2f;
        float halfY = mapY / 2f;
        float halfZ = mapZ / 2f;
        Vector3 worldCoord = new Vector3(gridCoord.x, gridCoord.y, gridCoord.z);
        worldCoord.x -= halfX;
        worldCoord.y -= halfY;
        worldCoord.z -= halfZ;
        worldCoord *= tileScale;
        return worldCoord;
    }

    public static IntVector3 GetBasicRoomDimensions(int minX, int minY, int minZ, int maxX, int maxY, int maxZ) {
        int x = Random.Range(minX, maxX);
        int y = Random.Range(minY, maxY);
        int z = Random.Range(minZ, maxZ);
        return new IntVector3(x, y, z);
    }

    public static float IntVector3Magnitude(IntVector3 v1, IntVector3 v2) {
        float x = Mathf.Pow(v1.x - v2.x, 2);
        float y = Mathf.Pow(v1.y - v2.y, 2);
        float z = Mathf.Pow(v1.z - v2.z, 2);
        return Mathf.Sqrt(x + y + z);
    }

    public static bool IntVector3Equals(IntVector3 v1, IntVector3 v2) {
        return v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
    }

    public static int IntVector3ManhattanDistance(IntVector3 v1, IntVector3 v2) {
        int distance = 0;
        distance += Mathf.Abs(v2.x - v1.x);
        distance += Mathf.Abs(v2.y - v1.y);
        distance += Mathf.Abs(v2.z - v1.z);
        return distance;
    }

    public static IntVector3[] GetLargestHistArea(List<IntVector3> intVector3s) {

        IntVector3[] corners = new IntVector3[2];

        // TODO
        // figure out rectangular area below

        HistVert[][][] histVerts = GetHistGraph(intVector3s);
        int max = 0;
        for(int i = 0; i < histVerts.Length; i++) {
            for(int j = 0; j < histVerts[i].Length; j++) {
                int temp = 0;
                for(int k = 0; k < histVerts[i][j].Length; k++) {
                    temp += histVerts[i][j][k].Value;
                }
                if(temp > max) {
                    max = temp;
                } else if(temp == max) {
                    
                }
            }
        }
        

        return corners;
    }

    private static HistVert[][][] GetHistGraph(List<IntVector3> intVector3s) {
        
        IntVector3 lowest = new IntVector3();
        IntVector3 highest = new IntVector3();
        lowest.x = LowestXWithinIntVector3List(intVector3s);
        lowest.y = LowestYWithinIntVector3List(intVector3s);
        lowest.z = LowestZWithinIntVector3List(intVector3s);
        highest.x = HighestXWithinIntVector3List(intVector3s);
        highest.y = HighestYWithinIntVector3List(intVector3s);
        highest.z = HighestZWithinIntVector3List(intVector3s);

        int offsetX = highest.x - lowest.x;
        int offsetY = highest.y - lowest.y;
        int offsetZ = highest.z - lowest.z;

        HistVert[][][] histVerts = new HistVert[offsetX][][];

        for (int x = lowest.x; x <= highest.x; x++) {
            if (histVerts[x] == null) {
                histVerts[x] = new HistVert[offsetY][];
            }
            for (int y = lowest.y; y <= highest.y; y++) {
                if (histVerts[y] == null) {
                    histVerts[y] = new HistVert[offsetZ][];
                }
                for (int z = lowest.z; z <= highest.z; z++) {
                    HistVert histVert = new HistVert {
                        x = x,
                        y = y,
                        z = z
                    };
                    IntVector3 position = new IntVector3(x, y, z);
                    if (intVector3s.Contains(position)) {
                        histVert.Value = 1;
                    }
                    histVerts[x - offsetX][y - offsetY][z - offsetZ] = histVert;
                }
            }
        }
        return histVerts;
    }

    private class HistVert {
        public int x;
        public int y;
        public int z;
        public int Value;
    }

    public static int LowestXWithinIntVector3List(List<IntVector3> intVector3s) {
        if(intVector3s.Count == 0) {
            Debug.LogWarning("IntVector3Builder: RECIEVED LIST WITH SIZE 0!");
            return -1;
        }
        int lowest = intVector3s[0].x;
        for(int i = 0; i < intVector3s.Count; i++) {
            if(intVector3s[i].x < lowest) {
                lowest = intVector3s[i].x;
            }
        }
        return lowest;
    }

    public static int LowestYWithinIntVector3List(List<IntVector3> intVector3s) {
        if (intVector3s.Count == 0) {
            Debug.LogWarning("IntVector3Builder: RECIEVED LIST WITH SIZE 0!");
            return -1;
        }
        int lowest = intVector3s[0].y;
        for (int i = 0; i < intVector3s.Count; i++) {
            if (intVector3s[i].y < lowest) {
                lowest = intVector3s[i].y;
            }
        }
        return lowest;
    }

    public static int LowestZWithinIntVector3List(List<IntVector3> intVector3s) {
        if (intVector3s.Count == 0) {
            Debug.LogWarning("IntVector3Builder: RECIEVED LIST WITH SIZE 0!");
            return -1;
        }
        int lowest = intVector3s[0].z;
        for (int i = 0; i < intVector3s.Count; i++) {
            if (intVector3s[i].z < lowest) {
                lowest = intVector3s[i].z;
            }
        }
        return lowest;
    }

    public static int HighestXWithinIntVector3List(List<IntVector3> intVector3s) {
        if (intVector3s.Count == 0) {
            Debug.LogWarning("IntVector3Builder: RECIEVED LIST WITH SIZE 0!");
            return -1;
        }
        int highest = intVector3s[0].x;
        for (int i = 0; i < intVector3s.Count; i++) {
            if (intVector3s[i].x > highest) {
                highest = intVector3s[i].x;
            }
        }
        return highest;
    }

    public static int HighestYWithinIntVector3List(List<IntVector3> intVector3s) {
        if (intVector3s.Count == 0) {
            Debug.LogWarning("IntVector3Builder: RECIEVED LIST WITH SIZE 0!");
            return -1;
        }
        int highest = intVector3s[0].y;
        for (int i = 0; i < intVector3s.Count; i++) {
            if (intVector3s[i].y > highest) {
                highest = intVector3s[i].y;
            }
        }
        return highest;
    }

    public static int HighestZWithinIntVector3List(List<IntVector3> intVector3s) {
        if (intVector3s.Count == 0) {
            Debug.LogWarning("IntVector3Builder: RECIEVED LIST WITH SIZE 0!");
            return -1;
        }
        int highest = intVector3s[0].z;
        for (int i = 0; i < intVector3s.Count; i++) {
            if (intVector3s[i].z > highest) {
                highest = intVector3s[i].z;
            }
        }
        return highest;
    }
}

[System.Serializable]
public struct IntVector3 {

    public IntVector3(int newX, int newY, int newZ) {
        x = newX;
        y = newY;
        z = newZ;
    }

    public static IntVector3 Forward = new IntVector3(0, 0, 1);
    public static IntVector3 Back = new IntVector3(0, 0, -1);
    public static IntVector3 Right = new IntVector3(1, 0, 0);
    public static IntVector3 Left = new IntVector3(-1, 0, 0);
    public static IntVector3 Up = new IntVector3(0, 1, 0);
    public static IntVector3 Down = new IntVector3(0, -1, 0);
    public static IntVector3 Zero = new IntVector3(0, 0, 0);

    public static bool operator ==(IntVector3 v1, IntVector3 v2) {
        return v1.Equals(v2);
    }
    public static bool operator !=(IntVector3 v1, IntVector3 v2) {
        return !v1.Equals(v2);
    }
    public static IntVector3 operator +(IntVector3 v1, IntVector3 v2) {
        return new IntVector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
    }
    public static IntVector3 operator -(IntVector3 v1, IntVector3 v2) {
        return new IntVector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
    }

    public int x;
    public int y;
    public int z;

    public override string ToString() {
        return $"IntVector3({x}, {y}, {z})";
    }
}
