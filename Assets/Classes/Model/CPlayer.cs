namespace Model {
	public class CPlayer {
		public bool isActive = false;
		public int matches = 0;
		public int score   = 0;

		public delegate void OnScoreChange(CPlayer player);

		public event OnScoreChange onScoreChange;

		public void AddScore (int scoreIncrease) {
			score += scoreIncrease;

			if (onScoreChange != null) {
				onScoreChange.Invoke(this);
			}
		}

		public bool IsStepFinished () {
			return isActive && matches == 0;
		}

		public bool CanSwipe () {
			return isActive && matches > 0;
		}
	}
}