using Libraries;
using Libraries.ActionSystem;
using Match;
using Match.Actions;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Match {
	public class CMatch : MonoBehaviour {
		public CMatcher mMatcher;
		public CView mView;
		public Game.CGame mGame;
		public Libraries.ActionSystem.CManager mActionManager;

		public Text playerText;
		public Text opponentText;
		public CPlayer player = new CPlayer();
		public CPlayer opponent = new CPlayer();

		public void OnRefreshEnd (IAction action) {
			var autoMatch = new CAutoMatch(mMatcher.FindMatches());
			mActionManager.AddAction(autoMatch);
		}

		public void OnMatchEnd (IAction action) {
			AddScore((action as Actions.CMatch).GetCountMatchIcon());
		}

		public void OnSwipeBegin (IAction action) {
			if (player.active) {
				player.matches--;
			} else {
				opponent.matches--;
			}
		}

		public void OnSwipeBackBegin (IAction action) {
			if (player.active) {
				player.matches++;
			} else {
				opponent.matches++;
			}
		}

		public void OnActionsFinished () {
			CRefreshPosition refresh = new CRefreshPosition(mView.mField);
			mActionManager.AddAction(refresh);

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

			mActionManager = new Libraries.ActionSystem.CManager();
			mActionManager.events.OnFinish(OnActionsFinished);

			mActionManager.events.On(EActionState.End  , (int) EEvents.RefreshPosition, OnRefreshEnd     );
			mActionManager.events.On(EActionState.End  , (int) EEvents.Match          , OnMatchEnd       );
			mActionManager.events.On(EActionState.Begin, (int) EEvents.Swipe          , OnSwipeBegin     );
			mActionManager.events.On(EActionState.Begin, (int) EEvents.SwipeBack      , OnSwipeBackBegin );

			mMatcher = new CMatcher(this);

			mView.init();

			var action = new Actions.CAutoMatch(mMatcher.FindMatches());
			mActionManager.AddAction(action);
		}

		void Update() {
			CGlobalUpdateManager.shared().doUpdate();
		}
	}
}