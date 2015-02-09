using UnityEngine;
using System.Collections;

namespace Match.Actions {
	public class CRefreshPosition : CBase, IIconMoveListener {
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
				move.SetListener(this);
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