using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorials/Actions/SpawnEnemy")]
public class TutorialActionSpawnEnemy : TutorialAction
{
    [SerializeField] private List<EnemySpawnData> _enemySpawnData = new List<EnemySpawnData>();

    public override TutorialActionStatus Execute() {
        for(int i = 0; i < _enemySpawnData.Count; i++) {
            EnemySpawn spawnPoint = LevelManager.CampaignLevelManagerInstance.GetEnemySpawnById(_enemySpawnData[i].EnemySpawnId);
            if (spawnPoint == null) {
                ErrorManager.LogError(nameof(TutorialActionSpawnEnemy), $"Could not retrieve enemy spawnpoint with id: {_enemySpawnData[i].EnemySpawnId}");
                return TutorialActionStatus.Abort;
            }
            EnemySpawnData enemySpawnData = _enemySpawnData[i];
            spawnPoint.SpawnNPC(enemySpawnData.InitialBrainStateId, enemySpawnData.InitialBrainStateDuration, enemySpawnData.OverrideEnemyUniqueId);
        }

        return base.Execute();
    }

    [System.Serializable]
    private class EnemySpawnData {
        [SerializeField] private string _enemySpawnId;
        [SerializeField] private string _overrideEnemyUniqueId;
        [SerializeField] private BrainStateTransitionId _initialBrainStateId;
        [SerializeField] private float _initialBrainStateDuration;

        public string EnemySpawnId => _enemySpawnId;
        public string OverrideEnemyUniqueId => _overrideEnemyUniqueId;
        public BrainStateTransitionId InitialBrainStateId => _initialBrainStateId;
        public float InitialBrainStateDuration => _initialBrainStateDuration;
    }
}
