using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanState : BrainState {

    [SerializeField] private BrainState _onEnemySeenState;

	public override void Execute() {
        if (_npcBehaviour.CheckVision()) {
            _npcBehaviour.ChangeBrainState(_onEnemySeenState);
        }
    }
}
