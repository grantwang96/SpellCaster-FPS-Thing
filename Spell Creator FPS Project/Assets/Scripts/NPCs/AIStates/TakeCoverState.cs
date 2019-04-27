using UnityEngine.AI;
using UnityEngine;

public class TakeCoverState : MoveState {

    private IVision _vision;

    public override void Enter(BrainState overrideBrainState = null) {
        base.Enter(overrideBrainState);
        _vision = _npcBehaviour.GetComponent<IVision>();

        if(_vision == null) {
            Debug.LogError("Unable to retrieve vision component on root object!");
            _npcBehaviour.ChangeBrainState(_onTargetReachedState);
            return;
        }

        NavMeshHit hit;
        if(NavMesh.FindClosestEdge(_moveController.transform.position, out hit, NavMesh.AllAreas)) {
            _moveController.SetDestination(hit.position);
            return;
        }
        _npcBehaviour.ChangeBrainState(_onTargetReachedState);
    }

    public override void Execute() {
        _moveController.SetRotation(_vision.CurrentTarget.BodyTransform.position);
    }
}
