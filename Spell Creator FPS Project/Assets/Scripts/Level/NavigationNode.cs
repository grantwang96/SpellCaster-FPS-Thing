using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Node that allows NPCMoveControllers to navigate around the level
/// </summary>
public class NavigationNode : MonoBehaviour {

    [SerializeField] private NavigationNode[] _neighbors;
    public NavigationNode[] Neighbors { get { return _neighbors; } }
    [SerializeField] private float _safeRadius; // used to check if object is on top of node

    private void Start() {
        NavigationNodeManager.Instance.RegisterNavigationNode(this);
    }

    public void RemoveNeighbor(NavigationNode neighbor) {
        List<NavigationNode> neighbors = new List<NavigationNode>(_neighbors);
        neighbors.Remove(neighbor);
        _neighbors = neighbors.ToArray();
    }

    public void RemoveAllNeighbors() {
        _neighbors = null;
    }

    public bool IsPathObstructed(Vector3 target) {
        Vector3 direction = target - transform.position;
        float distance = Vector3.Distance(target, transform.position);
        bool obstructed = Physics.Raycast(transform.position, direction, distance, NavigationNodeManager.Instance.NavigationNodeMask);
        Debug.Log($"Node {name} path obstructed to point {target}: {obstructed}");
        return obstructed;
    }

    public bool NeighborPathObstructed(NavigationNode neighbor) {
        Vector3 direction = neighbor.transform.position - transform.position;
        float distance = Vector3.Distance(neighbor.transform.position, transform.position);
        return Physics.Raycast(transform.position, direction, distance, NavigationNodeManager.Instance.NavigationNodeMask);
    }

    public bool IsNodeObstructed() {
        Vector3 center = transform.position + Vector3.up * _safeRadius * 0.5f;
        Collider[] coll = Physics.OverlapBox(center, new Vector3(_safeRadius, 2f, _safeRadius) * .5f, Quaternion.identity, NavigationNodeManager.Instance.NavigationNodeMask);
        return coll.Length != 0;
    }
}
