using Match.Gem;
using System.Collections;
using UnityEngine;

namespace Match.Actions {
	public class CRefreshPosition : CBase, IMoveObserver {
		public static IAction create() {
			return new CRefreshPosition();
		}

		public override bool validation() {
			return mIconField.HasEmptyIcon();
		}

		public override void startAction() {
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

		public void doUpdate() {}

		private void LaunchAutoMatch () {
			var action = mActionManager.createAction(EAction.eAutoMatch) as Actions.CAutoMatch;
			action.autoConfigure();
			mActionManager.AddAction(action);

			ComplateAction();
		}

		public override EEvents getActionEvent() {
			return EEvents.eRefreshPosition;
		}
	}

}