using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BrainState {

    protected NPCBehaviour npcBehaviour;

    public virtual string GetStateName() {
        return string.Empty;
    }

    public virtual void Enter(NPCBehaviour behaviour) {
        // Debug.Log(behaviour.name + " has entered " + ToString());
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

    public override string GetStateName() {
        return "Idle";
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
            npcBehaviour.ChangeBrainState(new MoveState(npcBehaviour.BaseSpeed));
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
        if (npcBehaviour.Agent.pathPending) { Debug.Log(npcBehaviour.name + " path is pending..."); return; }

        if (!npcBehaviour.Agent.hasPath) {
            Debug.LogWarning(npcBehaviour.name + " agent has no path!");
            npcBehaviour.ChangeBrainState(new IdleState(npcBehaviour.Blueprint.GetNewIdleTime));
            return;
        }
        facingTarget = npcBehaviour.CurrentTarget != null;
        Vector3 lookTarget = npcBehaviour.currentDestination;
        if (facingTarget) {
            lookTarget = npcBehaviour.CurrentTarget.transform.position;
        }

        npcBehaviour.Blueprint.OnMoveExecute(npcBehaviour, moveSpeed, lookTarget);
        CheckReachedPathCorner();
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
        if(npcBehaviour.CurrentTarget == null) {
            npcBehaviour.ChangeBrainState(new IdleState(npcBehaviour.Blueprint.GetNewIdleTime));
            return;
        }
        targetLastKnownPosition = behaviour.CurrentTarget.transform.position;
    }

    public override void Execute() {
        bool canSeeTarget = npcBehaviour.CanSeeTarget(npcBehaviour.CurrentTarget.GetBodyPosition());
        if (npcBehaviour.Blueprint.CanAttack(npcBehaviour)) {
            npcBehaviour.ChangeBrainState(new AttackState(0));
        }
        if (ReachedLastKnownDestination()) {
            if (!canSeeTarget) {
                // switch into attack mode
                npcBehaviour.ChangeBrainState(new MoveState(npcBehaviour.MaxSpeed));
                npcBehaviour.ClearCurrentTarget();
                return;
            }
        }
        if (canSeeTarget) {
            targetLastKnownPosition = npcBehaviour.CurrentTarget.transform.position;
        }
        npcBehaviour.Blueprint.OnChaseExecute(npcBehaviour, targetLastKnownPosition);
    }

    public override void Exit() {
        base.Exit();
    }

    private bool ReachedLastKnownDestination() {
        float distance = Vector3.Distance(npcBehaviour.transform.position, targetLastKnownPosition);
        return (distance < npcBehaviour.Agent.radius);
    }
}

/// <summary>
/// Runs for the duration of an attack animation
/// </summary>
public class AttackState : BrainState {

    private int _attackComboIndex;

    public AttackState(int attackComboIndex) : base() {
        _attackComboIndex = attackComboIndex;
    }

    public override string GetStateName() {
        return "Attack";
    }

    public override void Enter(NPCBehaviour behaviour) {
        base.Enter(behaviour);
        npcBehaviour.Blueprint.OnAttackEnter(npcBehaviour);
        if(_attackComboIndex >= npcBehaviour.Blueprint.AttackComboMax) { _attackComboIndex = 0; }
        npcBehaviour.CharacterAnimationHandler.SetIntParameter("AttackComboIndex", _attackComboIndex);
    }

    public override void Execute() {
        npcBehaviour.Blueprint.OnAttackExecute(npcBehaviour);
        float currentTime = npcBehaviour.CharacterAnimationHandler.GetCurrentAnimationTime();
        if(currentTime > 0.75f) {
            if (npcBehaviour.Blueprint.CanAttack(npcBehaviour)) {
                npcBehaviour.ChangeBrainState(new AttackState(_attackComboIndex + 1));
                return;
            }
        }
        if(currentTime >= 1f) {
            npcBehaviour.ChangeBrainState(new ChaseState());
        }
    }

    public override void Exit() {
        npcBehaviour.Blueprint.OnAttackExit(npcBehaviour);
    }
}
