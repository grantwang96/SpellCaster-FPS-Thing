
public class ArenaStats {

    public int Score { get; private set; }
	public int EnemiesDefeated { get; private set; }
    public int RoundsCompleted { get; private set; }

    public ArenaStats() {
        ArenaManager.ArenaManagerInstance.OnEnemyDefeated += OnEnemyDefeated;
        ArenaManager.ArenaManagerInstance.OnRoundEnded += OnRoundWon;
    }

    public void DeregisterListeners() {
        ArenaManager.ArenaManagerInstance.OnEnemyDefeated -= OnEnemyDefeated;
        ArenaManager.ArenaManagerInstance.OnRoundEnded -= OnRoundWon;
    }

    private void OnEnemyDefeated(int scoreValue) {
        EnemiesDefeated++;
        Score += scoreValue;
    }

    private void OnRoundWon(int round) {
        RoundsCompleted = round;
    }
}
