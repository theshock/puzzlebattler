using Libraries;
using Match;
using Match.Actions;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Match {
	public class CMatch : MonoBehaviour , INotificationObserver {
		public CNotificationManager mNotificationManager = new CNotificationManager();
		public CMatcher mSearcher;
		public CView mView;
		public Game.CGame mGame;
		public Game.CActionManager mActionManager;

		public Text playerText;
		public Text opponentText;
		public CPlayer player = new CPlayer();
		public CPlayer opponent = new CPlayer();

		public void handleNotification(int aEvent, Object aParam, CNotificationManager aManager) {
			if ((EEvents) aEvent == EEvents.Finish) {
				CRefreshPosition action = mActionManager.createAction(EAction.RefreshPosition) as CRefreshPosition;
				action.mIconField = mView.mField;
				mActionManager.AddAction(action);

				if (!mActionManager.HasActions()) {
					if (player.active && player.matches == 0) {
						player.active = false;
						opponent.active = true;
						opponent.matches = 3;
					} else if (opponent.active && opponent.matches == 0) {
						opponent.active = false;
						player.active = true;
						player.matches = 3;
					}

					if (opponent.active) {
						var searcher = new CSearcher(this);
						searcher.FindMoves();

						if (!searcher.MovesExists()) {
							Debug.Log("No moves");
							return;
						}

						var move = searcher.GetMoves()[0];
						mView.AiSwipe(move.from, move.to);
					}
				}
			}
		}

		public void AddScore (int count) {
			count = calculateMult(count);

			if (player.active) {
				player.AddScore(count);
				playerText.text = "" + player.score;
			} else {
				opponent.AddScore(count);
				opponentText.text = "" + opponent.score;
			}
		}

		private int calculateMult(int aCountMatch) {
			if (aCountMatch < 4) {
				return 3;
			} else {
				return ((aCountMatch - 1) * aCountMatch) / 2 - (aCountMatch - 3);
			}
		}

		void Start() {
			player.matches = 3;
			player.active = true;
			opponent.matches = 0;

			mActionManager = new Game.CActionManager();
			mActionManager.mMatchController = this;

			mNotificationManager.addObserver((int)EEvents.Finish, this);

			mSearcher = new CMatcher(this);

			mView.init();

			var action = mActionManager.createAction(EAction.AutoMatch) as Actions.CAutoMatch;
			action.autoConfigure();
			mActionManager.AddAction(action);
		}

		void Update() {
			CGlobalUpdateManager.shared().doUpdate();
		}
	}
}