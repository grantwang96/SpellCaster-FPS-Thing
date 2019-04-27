using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INavigationNodeManager {

    LayerMask NavigationNodeMask { get; }

    void RegisterNavigationNode(NavigationNode node);
    void DeregisterNavigationNode(NavigationNode node);
    NavigationNode GetClosestNode(Vector3 point);
}

public class NavigationNodeManager : MonoBehaviour, INavigationNodeManager {

    public static INavigationNodeManager Instance;

    [SerializeField] private List<NavigationNode> _navigationNodes = new List<NavigationNode>();
    [SerializeField] private LayerMask _navigationNodeMask;
    public LayerMask NavigationNodeMask => _navigationNodeMask;

    public void DeregisterNavigationNode(NavigationNode node) {
        _navigationNodes.Remove(node);
        for (int i = 0; i < node.Neighbors.Length; i++) {
            node.Neighbors[i].RemoveNeighbor(node);
        }
        node.RemoveAllNeighbors();
    }

    public void RegisterNavigationNode(NavigationNode node) {
        if (!_navigationNodes.Contains(node)) {
            _navigationNodes.Add(node);
        }
    }
    
    // TODO: make more efficient
    // Idea: Use path finding to find closest node(?)
    public NavigationNode GetClosestNode(Vector3 point) {
        if(_navigationNodes.Count == 0) {
            Debug.LogError("Navigation Nodes List is empty!");
            return null;
        }
        NavigationNode node = _navigationNodes[0];
        float originalDistance = Vector3.Distance(point, node.transform.position);
        for(int i = 1; i < _navigationNodes.Count; i++) {
            float distance = Vector3.Distance(point, _navigationNodes[i].transform.position);
            if(distance < originalDistance) {
                node = _navigationNodes[i];
                originalDistance = distance;
            }
        }
        return node;
    }

    public bool IsPathClear(Vector3 start, Vector3 end) {
        return Physics.Raycast(start, end - start, Vector3.Distance(start, end), NavigationNodeMask);
    }

    private void Awake() {
        Instance = this;
    }

    // Use this for initialization
    void Start () {
		
	}
}
