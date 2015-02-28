using Libraries.ActionSystem;
using Match.Actions;
using Model;
using UnityEngine;
using UnityEngine.UI;

namespace Match {
	public class CModel {

		CField field;

		public CModel (CField field) {
			this.field = field;

			var actionEvents = field.actions.events;

			actionEvents.OnFinish(OnActionsFinished);
			actionEvents.OnEnd  ((int) EEvents.RefreshPosition, OnRefreshEnd     );
			actionEvents.OnEnd  ((int) EEvents.Match          , OnMatchEnd       );
			actionEvents.OnBegin((int) EEvents.Swipe          , OnSwipeBegin     );
			actionEvents.OnBegin((int) EEvents.SwipeBack      , OnSwipeBackBegin );
		}

		public void OnRefreshEnd (IAction action) {
			var autoMatch = new CAutoMatch(field.FindMatches());
			field.actions.AddAction(autoMatch);
		}

		public void OnMatchEnd (IAction action) {
			AddScore((action as Actions.CMatch).GetCountMatchIcon());
		}

		public void OnSwipeBegin (IAction action) {
			Game.Instance.mModel.GetActivePlayer().matches--;
		}

		public void OnSwipeBackBegin (IAction action) {
			Game.Instance.mModel.GetActivePlayer().matches++;
		}

		public void OnActionsFinished () {
			CRefreshPosition refresh = new CRefreshPosition(field);
			field.actions.AddAction(refresh);

			if (!field.actions.HasActions()) {
				CheckActive();
				AiTurn();
			}
		}

		protected void CheckActive () {
			if (Game.Instance.mModel.GetActivePlayer().IsStepFinished()) {
				Game.Instance.mModel.SwitchPlayer();
			}
		}

		protected void AiTurn () {
			if (!Game.Instance.mModel.opponent.isActive) return;

			var searcher = new CSearcher(field);
			searcher.FindMoves();

			if (!searcher.MovesExists()) {
				Debug.Log("No moves");
				return;
			}

			var move = searcher.GetMoves()[0];

			field.actions.AddAction(new Actions.CSwipe(field, move.from, move.to));
		}

		public void AddScore (int count) {
			Text textNode = Game.Instance.mModel.player.isActive
				? field.playerText
				: field.opponentText;

			Game.Instance.mModel.GetActivePlayer().AddScore(CalculateScore(count));
			textNode.text = "" + Game.Instance.mModel.GetActivePlayer().score;
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