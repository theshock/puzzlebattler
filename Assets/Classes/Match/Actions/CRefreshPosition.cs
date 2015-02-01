using UnityEngine;
using System.Collections;

namespace Match.Actions {
	public class CRefreshPosition : CBase {
		private int mCountStartUpdateIcon = 0;

		public static IAction create() {
			return new CRefreshPosition();
		}


		public override bool validation() {
			return mIconField.HasEmptyIcon();
		}

		public override void startAction() {
			if (mIconField.updatePositionIcons(onEndMove) == 0) {
				LaunchAutoMatch();
			}
		}

		public void doUpdate() {}

		public void onEndMove() {
			LaunchAutoMatch();
		}

		private void LaunchAutoMatch () {
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