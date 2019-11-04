using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Objectives/DefeatSpecifiedEnemies")]
public class DefeatSpecifiedEnemiesObjectiveData : QuestObjectiveData{

    [SerializeField] private List<string> _enemiesToDefeat = new List<string>();

    public override QuestObjective GenerateQuestObjective() {
        DefeatSpecifiedEnemiesObjective objective = new DefeatSpecifiedEnemiesObjective(_enemiesToDefeat);
        return objective;
    }

    public class DefeatSpecifiedEnemiesObjective : QuestObjective {

        private List<string> _enemiesToDefeat = new List<string>();

        public DefeatSpecifiedEnemiesObjective(List<string> enemiesToDefeat) : base() {
            for (int i = 0; i < enemiesToDefeat.Count; i++) {
                _enemiesToDefeat.Add(enemiesToDefeat[i]);
            }
            NPCManager.Instance.OnEnemyDefeated += OnEnemyDefeated;
        }

        private void OnEnemyDefeated(EnemyBehaviour enemy) {
            _enemiesToDefeat.Remove(enemy.UniqueId);
            if (_enemiesToDefeat.Count == 0) {
                Completed = true;
                NPCManager.Instance.OnEnemyDefeated -= OnEnemyDefeated;
                FireObjectiveCompleted();
            }
        }
    }
}
