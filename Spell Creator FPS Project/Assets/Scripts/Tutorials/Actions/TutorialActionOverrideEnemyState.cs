using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Tutorials/Actions/OverrideEnemyState")]
public class TutorialActionOverrideEnemyState : TutorialAction
{
    [SerializeField] private string _enemyUniqueId;
    [SerializeField] private BrainStateTransitionId _transitionId;
    [SerializeField] private float _duration;

    public override TutorialActionStatus Execute() {
        EnemyBehaviour enemy = NPCManager.Instance.GetActiveNPC(_enemyUniqueId);
        if(enemy == null) {
            ErrorManager.LogError(nameof(TutorialActionOverrideEnemyState), $"Could not retrieve enemy with id: {_enemyUniqueId}");
            return TutorialActionStatus.Abort;
        }
        enemy.ChangeBrainState(_transitionId, _duration);
        return base.Execute();
    }
}
