namespace Model {
	public class CPlayer {
		public bool isActive = false;
		private int matches = 0;
		public int score   = 0;

		public delegate void OnScoreChange(CPlayer player, int scoreChange);

		public event OnScoreChange onScoreChange;

		private CGame game;

		public CPlayer(CGame game) {
			this.game = game;
		}

		public int GetMatches () {
			return matches;
		}

		public void SetMatches (int value) {
			matches = value;
		}

		public void UseMatch () {
			SetMatches(matches-1);
		}

		public void RestoreMatch () {
			SetMatches(matches+1);
		}

		public void AddScore (int scoreChange) {
			score += scoreChange;

			if (onScoreChange != null) {
				onScoreChange.Invoke(this, scoreChange);
			}
		}

		public bool IsStepFinished () {
			return isActive && (matches == 0 || game.timer.IsEnded());
		}

		public bool CanSwipe () {
			return isActive && matches > 0 && !game.timer.IsEnded();
		}
	}
}