using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BrainState {

    protected NPCBehaviour npcBehaviour;

    public virtual void Enter(NPCBehaviour behaviour) {
        Debug.Log(behaviour.name + " has entered " + ToString());
        npcBehaviour = behaviour;
    }
    public virtual void Execute() {
        // Apply NPC state update loop
    }
    public virtual void Exit() {
        // Apply any final changes/calculations before switching to new state
    }
}

public class TransitionState : BrainState {

    public TransitionState(float time) : base() {

    }
}

public class IdleState : BrainState {

    float idleTime;
    float idleStartTime;

    public IdleState(float length) : base() {
        idleTime = length;
    }
    
    public override void Enter(NPCBehaviour behaviour) {
        base.Enter(behaviour);
        idleStartTime = Time.time;
        npcBehaviour.Blueprint.OnIdleEnter(behaviour);
    }

    public override void Execute() {
        // idle time is over, change to walk mode
        if(Time.time - idleStartTime > idleTime) {
            npcBehaviour.targetDestination = npcBehaviour.GetNextDestination();
            npcBehaviour.ChangeBrainState(new MoveState(npcBehaviour.Blueprint.WalkSpeed));
        }
        // perform normal idle behavior
        npcBehaviour.Blueprint.OnIdleExecute(npcBehaviour);
    }
    
    public override void Exit() {
        npcBehaviour.Blueprint.OnIdleExit(npcBehaviour);
    }
}

public class MoveState : BrainState {

    float moveSpeed;
    bool facingTarget;

    public MoveState(float speed) : base() {
        moveSpeed = speed;
    }

    public override void Enter(NPCBehaviour behaviour) {
        base.Enter(behaviour);
        npcBehaviour.Blueprint.OnMoveEnter(npcBehaviour);
        if (!npcBehaviour.CalculatePath(npcBehaviour.targetDestination)) {
            npcBehaviour.ChangeBrainState(new IdleState(npcBehaviour.Blueprint.GetNewIdleTime));
            return;
        }
    }

    public override void Execute() {
        facingTarget = npcBehaviour.CurrentTarget != null;
        Vector3 lookDirection = npcBehaviour.transform.forward;

        if (npcBehaviour.Agent.pathPending) { Debug.Log(npcBehaviour.name + " path is pending..."); return; }

        if (!npcBehaviour.Agent.hasPath) {
            Debug.LogWarning(npcBehaviour.name + " agent has no path!");
            npcBehaviour.ChangeBrainState(new IdleState(npcBehaviour.Blueprint.GetNewIdleTime));
            return;
        }
        if (facingTarget) {
            lookDirection = npcBehaviour.CurrentTarget.transform.position - npcBehaviour.transform.position;
        } else {
            lookDirection = npcBehaviour.currentDestination - npcBehaviour.transform.position;
        }

        CheckReachedPathCorner();
        npcBehaviour.Blueprint.OnMoveExecute(npcBehaviour, moveSpeed, lookDirection);
    }

    public override void Exit() {
        npcBehaviour.Blueprint.OnMoveExit(npcBehaviour);
    }

    private void CheckReachedPathCorner() {
        float distanceFromCurrentDestination = Vector3.Distance(npcBehaviour.transform.position, npcBehaviour.currentDestination);
        if (distanceFromCurrentDestination < npcBehaviour.Agent.radius) {
            if (!npcBehaviour.NextPathCorner()) {
                npcBehaviour.ChangeBrainState(new IdleState(npcBehaviour.Blueprint.GetNewIdleTime));
            }
        }
    }
}

// movement specifically during combat
public class ChaseState : BrainState {

    Vector3 targetLastKnownPosition;

    public override void Enter(NPCBehaviour behaviour) {
        base.Enter(behaviour);
        if(npcBehaviour.CurrentTarget == null) { npcBehaviour.ChangeBrainState(new IdleState(npcBehaviour.Blueprint.GetNewIdleTime)); }
    }

    public override void Execute() {
        if (!npcBehaviour.CanSeeTarget(npcBehaviour.CurrentTarget.BodyTransform)) {
            Debug.Log("lost sight of target");
            npcBehaviour.ChangeBrainState(new MoveState(npcBehaviour.Blueprint.RunSpeed));
            return;
        }
        targetLastKnownPosition = npcBehaviour.CurrentTarget.transform.position;

        npcBehaviour.Blueprint.OnChaseExecute(npcBehaviour, targetLastKnownPosition);
    }

    public override void Exit() {
        base.Exit();
    }
}

/// <summary>
/// Runs for the duration of an attack animation
/// </summary>
public class AttackState : BrainState {

    public override void Enter(NPCBehaviour behaviour) {
        base.Enter(behaviour);
        npcBehaviour.Blueprint.OnAttackEnter(npcBehaviour);
    }

    public override void Execute() {
        npcBehaviour.Blueprint.OnAttackExecute(npcBehaviour);
    }

    public override void Exit() {
        npcBehaviour.Blueprint.OnAttackExit(npcBehaviour);
    }
}
