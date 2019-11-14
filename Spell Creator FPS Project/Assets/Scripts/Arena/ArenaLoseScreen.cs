using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArenaLoseScreen : LoseScreen {

    [SerializeField] private Text _totalScore;
    [SerializeField] private Text _enemiesDefeated;
    [SerializeField] private Text _roundsCompleted;
    
    public override void Initialize(UIPanelInitData initData) {
        base.Initialize(initData);
    }

    protected override void LoseScreenInit(UIPanelInitData initData) {
        ArenaLoseScreenInitData loseScreenData = initData as ArenaLoseScreenInitData;
        if(loseScreenData == null) {
            Debug.LogError($"[{nameof(ArenaLoseScreen)}] Did not receive init data of type {nameof(ArenaLoseScreenInitData)}");
            return;
        }
        SetTextValues(loseScreenData.ArenaStats);
    }

    private void SetTextValues(ArenaStats arenaStats) {
        _totalScore.text = $"Score: {arenaStats.Score}";
        _enemiesDefeated.text = $"Enemies Defeated: {arenaStats.EnemiesDefeated}";
        _roundsCompleted.text = $"Rounds Completed: {arenaStats.RoundsCompleted}";
    }

    protected override void RetryGame() {
        // close panels
        UIPanelManager.Instance.CloseUIPanel();
        // reload state
        GameManager.GameManagerInstance.CurrentSpellInventory?.ClearAll();
        GameManager.GameManagerInstance.CurrentRunicInventory?.ClearAll();
        GameStateManager.Instance.HandleTransition(GameplayValues.Navigation.ReenterArenaTransitionId);
    }

    protected override void QuitGame() {
        base.QuitGame();
        Debug.Log("Sucks to be you!");
    }
}

public class ArenaLoseScreenInitData : LoseScreenInitData {
    public ArenaStats ArenaStats;
}
