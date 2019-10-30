using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorials/Actions/SpawnEnemy")]
public class TutorialActionSpawnEnemy : TutorialAction
{
    [SerializeField] private string _enemySpawnId;
    [SerializeField] private string _overrideEnemyUniqueId;
    [SerializeField] private BrainStateTransitionId _initialBrainStateId;
    [SerializeField] private float _initialBrainStateDuration;

    public override TutorialActionStatus Execute() {
        EnemySpawn spawnPoint = LevelManager.CampaignLevelManagerInstance.GetEnemySpawnById(_enemySpawnId);
        if(spawnPoint == null) {
            ErrorManager.LogError(nameof(TutorialActionSpawnEnemy), $"Could not retrieve enemy spawnpoint with id: {_enemySpawnId}");
            return TutorialActionStatus.Abort;
        }
        spawnPoint.SpawnNPC(_initialBrainStateId, _initialBrainStateDuration, _overrideEnemyUniqueId);
        return base.Execute();
    }
}
