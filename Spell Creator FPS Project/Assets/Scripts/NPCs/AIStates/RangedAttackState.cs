using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackState : AttackState {

    [SerializeField] private BrainState _onTargetInRangeState;
    [SerializeField] private BrainState _onTargetOutOfRangeState;
}
