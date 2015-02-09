namespace Match {
	public class CPlayer {
		public int matches = 0;
		public bool active = false;
		public int score = 0;

		public void AddScore (int score) {
			this.score += score;
		}
	}
}