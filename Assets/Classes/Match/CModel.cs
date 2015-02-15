using Libraries.ActionSystem;
using Match.Actions;
using Model;
using UnityEngine;
using UnityEngine.UI;

namespace Match {
	public class CModel {

		CMatch controller;

		public CModel (CMatch controller) {
			this.controller = controller;

			var actionEvents = controller.mActionManager.events;

			actionEvents.OnFinish(OnActionsFinished);
			actionEvents.OnEnd  ((int) EEvents.RefreshPosition, OnRefreshEnd     );
			actionEvents.OnEnd  ((int) EEvents.Match          , OnMatchEnd       );
			actionEvents.OnBegin((int) EEvents.Swipe          , OnSwipeBegin     );
			actionEvents.OnBegin((int) EEvents.SwipeBack      , OnSwipeBackBegin );
		}

		public void OnRefreshEnd (IAction action) {
			var autoMatch = new CAutoMatch(controller.mMatcher.FindMatches());
			controller.mActionManager.AddAction(autoMatch);
		}

		public void OnMatchEnd (IAction action) {
			AddScore((action as Actions.CMatch).GetCountMatchIcon());
		}

		public void OnSwipeBegin (IAction action) {
			controller.mGame.mModel.GetActivePlayer().matches--;
		}

		public void OnSwipeBackBegin (IAction action) {
			controller.mGame.mModel.GetActivePlayer().matches++;
		}

		public void OnActionsFinished () {
			CRefreshPosition refresh = new CRefreshPosition(controller.mView.mField);
			controller.mActionManager.AddAction(refresh);

			if (!controller.mActionManager.HasActions()) {
				CheckActive();
				AiTurn();
			}
		}

		protected void CheckActive () {
			if (controller.mGame.mModel.GetActivePlayer().IsStepFinished()) {
				controller.mGame.mModel.SwitchPlayer();
			}
		}

		protected void AiTurn () {
			if (!controller.mGame.mModel.opponent.isActive) return;

			var searcher = new CSearcher(controller);
			searcher.FindMoves();

			if (!searcher.MovesExists()) {
				Debug.Log("No moves");
				return;
			}

			var move = searcher.GetMoves()[0];
			controller.mInput.StartSwipe(move.from, move.to);
		}

		public void AddScore (int count) {
			Text textNode = controller.mGame.mModel.player.isActive
				? controller.playerText
				: controller.opponentText;

			controller.mGame.mModel.GetActivePlayer().AddScore(CalculateScore(count));
			textNode.text = "" + controller.mGame.mModel.GetActivePlayer().score;
		}

		private int CalculateScore (int count) {
			if (count < 4) {
				return 3;
			} else {
				return ((count - 1) * count) / 2 - (count - 3);
			}
		}
	}
}