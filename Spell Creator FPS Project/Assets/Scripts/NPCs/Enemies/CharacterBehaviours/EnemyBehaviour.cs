using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : NPCBehaviour {

    protected override void Start() {
        base.Start();
        ChangeBrainState(new IdleState(blueprint.GetNewIdleTime));
    }
}
