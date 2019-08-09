using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationGod : MonoBehaviour {

    public static NavigationGod Instance;

    [SerializeField] private List<NavTriangle> _navTriangle = new List<NavTriangle>();

    private void Awake() {
        Instance = this;
    }
}

[System.Serializable]
public class NavTriangle {

    [SerializeField] private Transform _a;
    [SerializeField] private Transform _b;
    [SerializeField] private Transform _c;

    public Vector3 A => _a.position;
    public Vector3 B => _b.position;
    public Vector3 C => _c.position;

    // barycentric method picked up online
    public bool IsWithinTriangle(Vector3 point) {
        Vector3 v0 = C - A;
        Vector3 v1 = B - A;
        Vector3 v2 = point - A;

        float dot00 = Vector3.Dot(v0, v0);
        float dot01 = Vector3.Dot(v0, v1);
        float dot02 = Vector3.Dot(v0, v2);
        float dot11 = Vector3.Dot(v1, v1);
        float dot12 = Vector3.Dot(v1, v2);

        float invDenom = 1f / (dot00 * dot11 - dot01 * dot01);
        float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
        float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

        return (u >= 0) && (v >= 0) && (u + v < 1);
    }
}
