using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// listener object in level that sends tutorial triggers
public class TutorialEventListener : MonoBehaviour
{
    private const string EnemySpawnedTutorialTrigger = "EnemySpawned_";
    private const string EnemyDefeatedTutorialTrigger = "EnemyDefeated_";
    private const string UIPanelActivatedTutorialTrigger = "UIPanelActivated_";
    private const string UIPanelDeactivatedTutorialTrigger = "UIPanelDeactivated_";
    private const string QuestCompletedTutorialTrigger = "QuestCompleted_";

    public static TutorialEventListener Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        NPCManager.Instance.OnEnemySpawned += OnEnemySpawned;
        NPCManager.Instance.OnEnemyDefeated += OnEnemyDefeated;

        UIManager.Instance.OnPanelActivated += OnUIPanelActivated;
        UIManager.Instance.OnPanelDeactivated += OnUIPanelDeactivated;

        QuestManager.Instance.OnQuestCompleted += OnQuestCompleted;
    }

    private void OnDestroy() {
        NPCManager.Instance.OnEnemySpawned -= OnEnemySpawned;
        NPCManager.Instance.OnEnemyDefeated -= OnEnemyDefeated;

        UIManager.Instance.OnPanelActivated -= OnUIPanelActivated;
        UIManager.Instance.OnPanelDeactivated -= OnUIPanelDeactivated;

        QuestManager.Instance.OnQuestCompleted -= OnQuestCompleted;
    }

    private void OnEnemySpawned(string id, EnemyBehaviour enemy) {
        TutorialManager.Instance.FireTutorialTrigger($"{EnemySpawnedTutorialTrigger}{id}");
    }

    private void OnEnemyDefeated(EnemyBehaviour enemy) {
        TutorialManager.Instance.FireTutorialTrigger($"{EnemyDefeatedTutorialTrigger}{enemy.UniqueId}");
    }

    private void OnUIPanelActivated(string panel) {
        TutorialManager.Instance.FireTutorialTrigger($"{UIPanelActivatedTutorialTrigger}{panel}");
    }

    private void OnUIPanelDeactivated(string panel) {
        TutorialManager.Instance.FireTutorialTrigger($"{UIPanelDeactivatedTutorialTrigger}{panel}");
    }

    private void OnQuestCompleted(Quest quest) {
        TutorialManager.Instance.FireTutorialTrigger($"{QuestCompletedTutorialTrigger}{quest.Id}");
    }
}
