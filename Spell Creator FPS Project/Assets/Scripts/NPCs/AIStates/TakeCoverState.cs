using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TakeCoverState : MoveState {

    // configurations for acquiring new cover destination
    [SerializeField] private float _searchRadiusOuter;
    [SerializeField] private float _searchRadiusInner;
    [SerializeField] private int _searchQueries;

    [SerializeField] private NPCVision _vision;
    private bool _canSeeTarget;
    
    protected override Vector3 GetDestination() {
        /*
        Vector3 destination = _moveController.transform.position;
        Vector3 targetDir = _vision.CurrentTarget.transform.position - _moveController.transform.position;
        Vector3 position = _moveController.transform.position + -targetDir.normalized * _searchRadiusOuter;
        NavMeshHit hit;
        if (NavMesh.FindClosestEdge(position, out hit, NavMesh.AllAreas)) {
            destination = hit.position;
        }
        */
        Vector3 destination = _vision.CurrentTarget.transform.position;
        NavMeshHit hit;
        if(NavMesh.FindClosestEdge(destination, out hit, NavMesh.AllAreas)) {
            destination = hit.position;
        }
        return destination;
    }

    protected override void SetRotation() {
        _moveController.SetRotation(_vision.CurrentTarget.transform.position);
    }

    protected override void OnArriveDestination() {
        // select a state
        BrainState nextState = _onTargetReachedStates[Random.Range(0, _onTargetReachedStates.Length)];
        _npcBehaviour.ChangeBrainState(nextState);
        _moveController.ClearCurrentDestination();
    }

    protected override void OnEnterHitStun(Vector3 direction, float power) {
        _npcBehaviour.ChangeBrainState(BrainStateTransitionId.Alert, this);
    }
}
