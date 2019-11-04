using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Quest
{
    // list of objectives
    private List<QuestObjective> _objectives = new List<QuestObjective>();

    public readonly string Id;
    public bool Completed { get; private set; }
    
    public event Action<Quest> OnQuestStateUpdated;

    public Quest(string id, List<QuestObjective> objectives) {
        Id = id;
        for(int i = 0; i < objectives.Count; i++) {
            _objectives.Add(objectives[i]);
            objectives[i].OnObjectiveCompleted += OnQuestObjectiveUpdated;
        }
    }

    private void OnQuestObjectiveUpdated() {
        Completed = true;
        for(int i = 0; i < _objectives.Count; i++) {
            if (!_objectives[i].Completed) {
                Completed = false;
                break;
            }
        }
        OnQuestStateUpdated?.Invoke(this);
    }
}