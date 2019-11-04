using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IQuestManager {
    void RegisterQuest(Quest quest);

    event Action<Quest> OnQuestCompleted;
}

public class QuestManager : MonoBehaviour, IQuestManager
{
    public static IQuestManager Instance;

    private List<Quest> _activeQuests = new List<Quest>();

    public event Action<Quest> OnQuestCompleted;

    private void Awake() {
        Instance = this;
    }

    private void OnQuestStateUpdated(Quest quest) {
        if (quest.Completed) {
            OnQuestCompleted?.Invoke(quest);
            _activeQuests.Remove(quest);
        }
    }

    public void RegisterQuest(Quest quest) {
        if (_activeQuests.Contains(quest)) {
            return;
        }
        _activeQuests.Add(quest);
        quest.OnQuestStateUpdated += OnQuestStateUpdated;
    }
}
