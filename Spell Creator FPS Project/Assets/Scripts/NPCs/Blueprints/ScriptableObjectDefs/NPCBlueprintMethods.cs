using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract partial class NPCBlueprint : ScriptableObject {

    // these functions are called by brain states. BrainStates handle duration/timing
    public virtual void OnIdleEnter(NPCBehaviour npc) {
        // play idle animation
    }
    public virtual void OnIdleExecute(NPCBehaviour npc) {
        npc.CheckVision();
    }
    public virtual void OnIdleExit(NPCBehaviour npc) {

    }

    public virtual void OnMoveEnter(NPCBehaviour npc) {

    }
    public virtual void OnMoveExecute(NPCBehaviour npc, float speed, Vector3 target) {
        npc.CheckVision();
        Vector3 dir = npc.currentDestination - npc.transform.position;
        dir.y = 0;
        npc.CharMove.Move(dir, target, speed);
    }
    public virtual void OnMoveExit(NPCBehaviour npc) {
        npc.CharMove.Stop();
    }

    public virtual void OnChaseEnter(NPCBehaviour npc) {

    }
    public virtual void OnChaseExecute(NPCBehaviour npc, Vector3 target) {
        Vector3 dir = target - npc.transform.position;
        dir.y = 0;
        npc.CharMove.Move(dir, target, RunSpeed);
    }
    public virtual void OnChaseExit(NPCBehaviour npc) {
        npc.CharMove.Stop();
    }

    public virtual void OnAttackEnter(NPCBehaviour npc) {
        npc.CharMove.Stop();
        npc.CharMove.SetRotation(npc.CurrentTarget.transform.position);
    }
    public virtual void OnAttackExecute(NPCBehaviour npc) {

    }
    public virtual void OnAttackExit(NPCBehaviour npc) {

    }
    public virtual bool CanAttack(NPCBehaviour npc) {
        if(npc.CurrentTarget == null) { return false; }
        float distance = Vector3.Distance(npc.transform.position, npc.CurrentTarget.transform.position);
        return distance <= AttackRange;
    }

    public virtual void OnSkillUseEnter(NPCBehaviour npc) {

    }
    public virtual void OnSkillUseExecute(NPCBehaviour npc) {

    }
    public virtual void OnSkillUseExit(NPCBehaviour npc) {

    }
}
