using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestObjective
{
    public bool Completed { get; protected set; }
    public event Action OnObjectiveCompleted;

    protected void FireObjectiveCompleted() {
        OnObjectiveCompleted?.Invoke();
    }
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
