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
        _isGrounded = _characterBehaviour.MoveController.IsGrounded;
        ProcessMovement();
        UpdateVariables();
    }

    private void ProcessMovement() {
        if (!_isGrounded) {
            _moveMagnitude = 0f;
            return;
        }
        _moveMagnitude = _characterBehaviour.MoveController.MoveSpeed / _characterBehaviour.MoveController.MaxSpeed;
    }

    private void UpdateVariables() {
        _anim.SetBool("IsGrounded", _isGrounded);
        _anim.SetFloat("Move", _moveMagnitude);
    }
}
