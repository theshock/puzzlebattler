namespace Model {
	public class CPlayer {
		public bool isActive = false;
		private int matches = 0;
		public int damage = 0;
		public int max = 0;

		public delegate void OnScoreChange(CPlayer player, int scoreChange);
		public delegate void OnMatchesChange(CPlayer player);

		public event OnScoreChange onDamageChange;
		public event OnMatchesChange onMatchesChange;

		private CGame game;

		public CPlayer(CGame game) {
			this.game = game;
		}

		public void SetMaxDamage(int value) {
			max = value;
		}

		public float GetLives() {
			return (max - damage) / (float) max;
		}

		public int GetMatches () {
			return matches;
		}

		public int GetRealtimeMatches () {
			return isActive ? matches : 0;
		}

		public void SetMatches (int value) {
			matches = value;
			onMatchesChange(this);
		}

		public void UseAction() {
			SetMatches(matches-1);
		}

		public void RestoreAction() {
			SetMatches(matches+1);
		}

		public void AddDamage(int change) {
			damage += change;

			if (damage < 0) damage = 0;

			if (onDamageChange != null) {
				onDamageChange.Invoke(this, change);
			}
		}

		public bool IsStepFinished () {
			return isActive && (matches == 0 || game.timer.IsEnded());
		}

		public bool CanAction() {
			return isActive
				&& matches > 0
				&& !game.timer.IsEnded()
				&& game.playerCharacter.IsIdle()
				&& game.opponentCharacter.IsIdle()
				&& game.match.actions.HasActions() == false
				&& GetLives() > 0;
		}
	}
}