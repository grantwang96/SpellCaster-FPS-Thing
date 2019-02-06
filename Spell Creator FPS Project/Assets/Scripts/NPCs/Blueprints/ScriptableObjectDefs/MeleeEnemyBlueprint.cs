using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "NPCBlueprint/MeleeEnemy")]
public class MeleeEnemyBlueprint : NPCBlueprint {

    public override void OnAttackEnter(NPCBehaviour npc) {
        base.OnAttackEnter(npc);
    }

    public override void OnAttackExecute(NPCBehaviour npc) {
        base.OnAttackExecute(npc);
    }

    public override void OnAttackExit(NPCBehaviour npc) {
        base.OnAttackExit(npc);
    }
}
