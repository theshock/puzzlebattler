using Match.Gem;
using System.Collections;
using UnityEngine;

namespace Match.Actions {
	public class CRefreshPosition : CBase, IMoveObserver {
		protected CField mIconField;

		public CRefreshPosition (CField field) {
			mIconField = field;
		}

		public override bool Validation() {
			return mIconField.HasEmptyIcon();
		}

		public override void StartAction() {
			var move = mIconField.UpdatePositionIcons();

			if (move.IsFinished()) {
				LaunchAutoMatch();
			} else {
				move.SetObserver(this);
			}
		}

		public void OnMoveEnd () {
			LaunchAutoMatch();
		}

		private void LaunchAutoMatch () {
			var action = new CAutoMatch(mActionManager.mMatchController.mSearcher.FindMatches());
			mActionManager.AddAction(action);

			ComplateAction();
		}

		public override EEvents GetActionEvent() {
			return EEvents.RefreshPosition;
		}
	}

}