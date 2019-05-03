﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TakeCoverState : MoveState {

    // configurations for acquiring new cover destination
    [SerializeField] private float _searchRadiusOuter;
    [SerializeField] private float _searchRadiusInner;
    [SerializeField] private int _searchQueries;

    private IVision _vision;

    private void Awake() {
        _vision = _npcBehaviour.GetComponent<IVision>();
    }

    protected override Vector3 GetDestination() {
        Vector3 destination = _moveController.transform.position;
        Vector3 targetDir = _vision.CurrentTarget.transform.position - _moveController.transform.position;
        Vector3 position = _moveController.transform.position + -targetDir.normalized * _searchRadiusOuter;
        NavMeshHit hit;
        if (NavMesh.FindClosestEdge(position, out hit, NavMesh.AllAreas)) {
            /*
            float dotProd = Vector3.Dot(targetDir, hit.normal);
            if (dotProd <= 0f) {
            }
            */
            destination = hit.position;
        }
        return destination;
    }

    protected override void SetRotation() {
        _moveController.SetRotation(_vision.CurrentTarget.transform.position);
    }
}
