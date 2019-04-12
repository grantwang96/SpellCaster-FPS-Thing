using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract partial class NPCBlueprint : ScriptableObject {

    public virtual bool CanAttack(NPCBehaviour npc) {
        if(npc.CurrentTarget == null) { return false; }
        float distance = Vector3.Distance(npc.transform.position, npc.CurrentTarget.transform.position);
        return distance <= AttackRange;
    }

    public virtual bool IsEnemy(CharacterBehaviour characterBehaviour) {
        for(int i = 0; i < characterBehaviour.UnitTags.Count; i++) {
            if(ArrayHelper.Contains(_enemyTags, characterBehaviour.UnitTags[i])) {
                Debug.Log($"{characterBehaviour.name} is an enemy!");
                return true;
            }
        }
        return false;
    }
}
