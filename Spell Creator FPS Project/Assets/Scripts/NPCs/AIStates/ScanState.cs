using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanState : BrainState {

    [SerializeField] private BrainState _onEnemySeenState;
    
    public override void Execute() {
        if(_npcVision == null) {
            return;
        }
        CharacterBehaviour seenCharacter = _npcVision.CheckVision();
        if (seenCharacter) {
            _npcVision.SetCurrentTarget(seenCharacter);
            _npcBehaviour.ChangeBrainState(_onEnemySeenState);
        }
    }
}
