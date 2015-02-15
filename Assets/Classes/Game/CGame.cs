using Libraries;
using Model;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game {
	public class CGame : MonoBehaviour {
		public Config.CConfig mConfig;
		public Model.CGame mModel;
		public Match.CMatch mMatch;

		public Image playerValue;
		public Image opponentValue;

		public void Start () {
			mModel = new Model.CGame(mConfig);

			mModel.player  .onScoreChange += OnScoreChange;
			mModel.opponent.onScoreChange += OnScoreChange;

			Update();
		}

		public void OnScoreChange (CPlayer player, int scoreChange) {
			Update();
		}

		public void Update () {
			Count(mModel.player, mConfig.mMatch.mOpponent.mHealth, opponentValue);
			Count(mModel.opponent, mConfig.mMatch.mPlayer.mHealth, playerValue);
		}

		protected void Count(CPlayer player, int maxHealth, Image image) {
			var currentHealth = (maxHealth - player.score);
			if (currentHealth < 0) {
				currentHealth = 0;
			}

			float bar = currentHealth / (float) maxHealth;

			var rect = image.GetComponent<RectTransform>();
			image.GetComponent<RectTransform>().sizeDelta = new Vector2(24 + bar * 300, rect.sizeDelta.y);

			image.GetComponent<Image>().color = new Color(
				bar >= 0.5f ? (1 - bar) * 2 : 1,
				bar <= 0.5f ?      bar  * 2 : 1,
				0
			);
}
	}
}