using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAnimController : CharacterAnimationHandler {

    protected virtual void Update() {
        float move = characterBehaviour.GetMoveMagnitude();
        anim.SetFloat("Move", move);
    }

	protected void OnNPCIdle() {
        // anim.SetTrigger("Attack");
    }
}
