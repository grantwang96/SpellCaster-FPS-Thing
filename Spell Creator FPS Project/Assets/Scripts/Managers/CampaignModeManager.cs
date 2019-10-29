using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICampaignModeManager {

}

public class CampaignModeManager : GameManager
{
    public static ICampaignModeManager ManagerInstance;

    private CampaignState _currentState;

    private Damageable _bossDamageable;
    
    protected override void SubscribeToEvents() {
        base.SubscribeToEvents();
        if (_bossDamageable != null) {
            _bossDamageable.OnDeath += OnBossDefeated;
        }
    }

    protected override void UnsubscribeToEvents() {
        base.UnsubscribeToEvents();
        if (_bossDamageable != null) {
            _bossDamageable.OnDeath -= OnBossDefeated;
        }
    }

    private void ChangeState(CampaignState newCampaignState) {
        if(_currentState != null) {
            _currentState.Exit();
        }
        _currentState = newCampaignState;
        _currentState.Enter(this);
    }

    protected override void OnPlayerDefeated(bool isDead, Damageable damageable) {
        ChangeState(new CampaignLoseState());
    }

    private void OnBossDefeated(bool isDead, Damageable damageable) {
        ChangeState(new CampaignWonState());
    }

    protected override void OnCurrentGameStateExited() {
        base.OnCurrentGameStateExited();
        _currentState.OnGameModeFinish();
    }

    private class CampaignState {

        protected CampaignModeManager _manager;

        public virtual void Enter(CampaignModeManager manager) {
            _manager = manager;
            // have child states set some UI to reflect current state
        }

        public virtual void OnGameModeFinish() {
            
        }

        public virtual void Exit() {

        }
    }

    private class CampaignWonState : CampaignState {
        public override void OnGameModeFinish() {
            AddSpellsToPersistedInventory();
            AddRunesToPersistedInventory();
        }

        private void AddRunesToPersistedInventory() {
            IRunicInventory runicInventory = GameManagerInstance.CurrentRunicInventory;
            IReadOnlyList<KeyValuePair<string, int>> allRunes = runicInventory.RetrieveAllItems();
            for(int i = 0; i < allRunes.Count; i++) {
                PersistedInventory.RunicInventory.AddItem(allRunes[i].Key, allRunes[i].Value);
            }
        }

        private void AddSpellsToPersistedInventory() {
            ISpellInventory spellInventory = GameManagerInstance.CurrentSpellInventory;
            IReadOnlyList<StorableSpell> allSpells = spellInventory.StoredSpells;
            for (int i = 0; i < allSpells.Count; i++) {
                PersistedInventory.SpellInventory.AddSpell(allSpells[i]);
            }
        }
    }

    private class CampaignLoseState : CampaignState{

    }
}
