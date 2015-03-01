namespace Model {
	public class CModel {

		public CPlayer player;
		public CPlayer opponent;

		public CModel() {
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
			if (CGame.Config.match.isPlayerFirst) {
				ActivatePlayer(player);
			} else  {
				ActivatePlayer(opponent);
			}
		}

		public void ActivatePlayer(CPlayer active) {
			if (active == player) {
				player.isActive = true;
				player.matches = CGame.Config.match.player.matchesPerTurn;
				opponent.isActive = false;
			} else {
				opponent.isActive = true;
				opponent.matches = CGame.Config.match.opponent.matchesPerTurn;
				player.isActive = false;
			}
		}

	}
}