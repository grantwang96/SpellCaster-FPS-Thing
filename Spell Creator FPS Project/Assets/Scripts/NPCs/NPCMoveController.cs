﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMoveController : CharacterMoveController {

    protected CharacterBehaviour characterBehaviour; // gain read access from character's brain

    // move the character at this speed in this direction
    public virtual void Move(Vector3 moveDir, Vector3 target, float speed) {
        if (!performingAction) {
            moveDir = moveDir.normalized;
            movementVelocity.x = moveDir.x * speed;
            movementVelocity.z = moveDir.z * speed;

            SetRotation(target);
        }
    }

    // sets the rotation of the character
    public virtual void SetRotation(Vector3 target) {
        Vector3 lookDirection = target - transform.position;
        Vector3 lookDirectionBody = lookDirection;
        lookDirectionBody.y = 0;

        // TODO: set head to lookDirection
        characterBehaviour.Head.forward = lookDirection;

        // set root to lookDirectionBody;
        transform.forward = lookDirectionBody;
    }
}
