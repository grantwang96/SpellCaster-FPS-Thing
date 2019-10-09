
public class ArenaStats {

    public int Score { get; private set; }
	public int EnemiesDefeated { get; private set; }
    public int RoundsCompleted { get; private set; }

    public ArenaStats() {
        ArenaManager.Instance.OnEnemyDefeated += OnEnemyDefeated;
        ArenaManager.Instance.OnRoundEnded += OnRoundWon;
    }

    public void DeregisterListeners() {
        ArenaManager.Instance.OnEnemyDefeated -= OnEnemyDefeated;
        ArenaManager.Instance.OnRoundEnded -= OnRoundWon;
    }

    private void OnEnemyDefeated(int scoreValue) {
        EnemiesDefeated++;
        Score += scoreValue;
    }

    private void OnRoundWon(int round) {
        RoundsCompleted = round;
    }
}
