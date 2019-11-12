using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAnimController : CharacterAnimationHandler {

    protected bool _isGrounded;
    protected float _moveMagnitude;
    protected string _stateId;

    protected override void Start() {
        base.Start();
    }

    protected virtual void Update() {
        ProcessMovement();
        UpdateVariables();
    }

    private void ProcessMovement() {
        _isGrounded = _moveController.IsGrounded;
        if (!_isGrounded) {
            _moveMagnitude = 0f;
            return;
        }
        _moveMagnitude = GetMoveMagnitude() / _characterBehaviour.MaxSpeed;
    }

    private float GetMoveMagnitude() {
        Vector3 vel = _moveController.MovementVelocity;
        vel.y = 0f;
        return vel.magnitude;
    }

    private void UpdateVariables() {
        _anim.SetBool("IsGrounded", _isGrounded);
        _anim.SetFloat("Move", _moveMagnitude);
    }
}
