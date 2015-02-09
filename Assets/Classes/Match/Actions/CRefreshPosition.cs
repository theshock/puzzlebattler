using UnityEngine;
using System.Collections;

namespace Match.Actions {
	public class CRefreshPosition : CBase {
		public static IAction create() {
			return new CRefreshPosition();
		}

		public override bool validation() {
			return mIconField.HasEmptyIcon();
		}

		public override void startAction() {
			mIconField.mOnIconsMoveEnd += LaunchAutoMatch;

			if (!mIconField.UpdatePositionIcons()) {
				LaunchAutoMatch();
			}
		}

		public void doUpdate() {}

		private void LaunchAutoMatch () {
			mIconField.mOnIconsMoveEnd -= LaunchAutoMatch;

			var action = mActionManager.createAction(EAction.eAutoMatch) as Actions.CAutoMatch;
			action.autoConfigure();
			mActionManager.addAction(action);

			complateAction();
		}

		public override EEvents getActionEvent() {
			return EEvents.eRefreshPosition;
		}
	}

}