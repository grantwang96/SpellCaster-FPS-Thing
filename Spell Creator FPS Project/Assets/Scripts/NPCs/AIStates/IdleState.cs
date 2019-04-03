using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BrainState {

    [SerializeField] private BrainState _onIdleEnd;
    [SerializeField] private float idleTime;

    [SerializeField] private NPCMoveController _moveController;

    private float idleStartTime;

    public override void Enter(NPCBehaviour behaviour) {
        base.Enter(behaviour);
        idleStartTime = Time.time;
        _npcBehaviour.CharMove.Stop();
        // _npcBehaviour.Blueprint.OnIdleEnter(behaviour);
    }

    public override void Execute() {
        // idle time is over, change to walk mode
        if (Time.time - idleStartTime > idleTime) {
            _npcBehaviour.ChangeBrainState(_onIdleEnd);
        }

        base.Execute();
        // perform normal idle behavior
        // _npcBehaviour.Blueprint.OnIdleExecute(_npcBehaviour);
    }

    public override void Exit() {
        base.Exit();
        // _npcBehaviour.Blueprint.OnIdleExit(_npcBehaviour);
    }
}
