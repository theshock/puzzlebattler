namespace Model {
	public class CGame {

		public CPlayer player;
		public CPlayer opponent;

		protected Config.Match.CMatch config;

		public CGame (Config.CConfig config) {
			this.config = config.mMatch;

			player   = new CPlayer();
			opponent = new CPlayer();
		}

		public CPlayer GetActivePlayer () {
			return player.isActive ? player : opponent;
		}

		public void SwitchPlayer () {
			if (player.isActive) {
				ActivatePlayer(opponent);
			} else {
				ActivatePlayer(player);
			}
		}

		public void ActivateFirst () {
			if (config.mIsPlayerFirst) {
				ActivatePlayer(player);
			} else  {
				ActivatePlayer(opponent);
			}
		}

		public void ActivatePlayer(CPlayer active) {
			if (active == player) {
				player.isActive = true;
				player.matches = config.mPlayer.mMatches;
				opponent.isActive = false;
			} else {
				opponent.isActive = true;
				opponent.matches = config.mOpponent.mMatches;
				player.isActive = false;
			}
		}

	}
}