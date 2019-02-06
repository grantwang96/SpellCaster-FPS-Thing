using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMoveController : CharacterMoveController {

    [SerializeField] protected float _turnSpeed;
    protected NPCBehaviour npcBehaviour; // gain read access from character's brain

    protected override void Awake() {
        base.Awake();
        npcBehaviour = GetComponent<NPCBehaviour>();
    }

    protected override void Start() {
        base.Start();

        _baseSpeed = npcBehaviour.Blueprint.WalkSpeed;
        _maxSpeed = npcBehaviour.Blueprint.RunSpeed;
    }

    // move the character at this speed in this direction
    public virtual void Move(Vector3 moveDir, Vector3 target, float speed) {
        if (externalForces == null) {
            moveDir = moveDir.normalized;
            movementVelocity.x = moveDir.x * speed;
            movementVelocity.z = moveDir.z * speed;
            SetRotation(target);
        }
    }

    public virtual void Stop() {
        movementVelocity.x = 0f;
        movementVelocity.z = 0f;
    }

    // sets the rotation of the character
    public virtual void SetRotation(Vector3 target) {
        Vector3 lookDirection = target - npcBehaviour.Head.position;
        Vector3 lookDirectionBody = lookDirection;
        lookDirectionBody.y = 0;

        float step = _turnSpeed * Time.deltaTime;
        float radStep = Mathf.Deg2Rad * step;
        Vector3 newLook = Vector3.RotateTowards(transform.forward, lookDirectionBody, radStep, 0f);
        transform.rotation = Quaternion.LookRotation(newLook);
    }
}
