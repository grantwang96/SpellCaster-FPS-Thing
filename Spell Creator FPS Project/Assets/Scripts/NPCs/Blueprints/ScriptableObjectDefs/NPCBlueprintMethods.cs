using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract partial class NPCBlueprint : ScriptableObject {
    
    public virtual bool IsEnemy(CharacterBehaviour characterBehaviour) {
        for(int i = 0; i < characterBehaviour.UnitTags.Count; i++) {
            if(ArrayHelper.Contains(_enemyTags, characterBehaviour.UnitTags[i])) {
                return true;
            }
        }
        return false;
    }
}
