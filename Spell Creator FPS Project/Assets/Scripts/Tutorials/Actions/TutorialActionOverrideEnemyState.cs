using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Tutorials/Actions/OverrideEnemyState")]
public class TutorialActionOverrideEnemyState : TutorialAction
{
    [SerializeField] private EnemyStateOverrideData[] _enemyStateOverrideDatas;

    public override TutorialActionStatus Execute() {
        for(int i = 0; i < _enemyStateOverrideDatas.Length; i++) {
            EnemyBehaviour enemy = NPCManager.Instance.GetActiveNPC(_enemyStateOverrideDatas[i].EnemyUniqueId);
            if (enemy == null) {
                ErrorManager.LogError(nameof(TutorialActionOverrideEnemyState), $"Could not retrieve enemy with id: {_enemyStateOverrideDatas[i].EnemyUniqueId}");
                return TutorialActionStatus.Abort;
            }
            enemy.ChangeBrainState(_enemyStateOverrideDatas[i].TransitionId, null, _enemyStateOverrideDatas[i].Duration);
        }
        return base.Execute();
    }

    [System.Serializable]
    private class EnemyStateOverrideData {
        [SerializeField] private string _enemyUniqueId;
        [SerializeField] private BrainStateTransitionId _transitionId;
        [SerializeField] private float _duration;

        public string EnemyUniqueId => _enemyUniqueId;
        public BrainStateTransitionId TransitionId => _transitionId;
        public float Duration => _duration;
    }
}
