using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelBuildHelper {

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

    public static IntVector3 IntVector3Sum(IntVector3 v1, IntVector3 v2) {
        return new IntVector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
    }

    public static IntVector3 IntVector3Diff(IntVector3 v1, IntVector3 v2) {
        return new IntVector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
    }
}

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

    public int x;
    public int y;
    public int z;
}
