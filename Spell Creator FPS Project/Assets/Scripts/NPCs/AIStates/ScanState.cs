using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanState : BrainState {

    [SerializeField] private BrainState _onEnemySeenState;

    private IVision _vision;

    private void Awake() {
        _vision = _npcBehaviour.GetComponent<IVision>();
        if(_vision == null) {
            Debug.LogError($"{name}'s Scan State did not find IVision component!");
        }
    }

    protected override void SetTriggerName() {
        _triggerName = GameplayValues.BrainStates.ScanStateId;
    }

    public override void Execute() {
        if (_vision.CheckVision()) {
            _npcBehaviour.ChangeBrainState(_onEnemySeenState);
        }
    }
}
