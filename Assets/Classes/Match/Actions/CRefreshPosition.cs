using UnityEngine;
using System.Collections;

namespace Match.Actions {
	public class CRefreshPosition : CBase {
		private int mCountStartUpdateIcon = 0;

		public static IAction create() {
			return new CRefreshPosition();
		}


		public override bool validation() {
			return mIconField.isHaveEmptyIcon();
		}

		public override void startAction() {
			mCountStartUpdateIcon = mIconField.updatePositionIcons(onEndMove);

			if (mCountStartUpdateIcon == 0) {
				IAction action = mActionManager.createAction(EAction.eAutoMatch);
				mActionManager.addAction(action);

				complateAction();
			}
		}

		public void doUpdate() {}

		public void onEndMove() {
			IAction action = mActionManager.createAction(EAction.eAutoMatch);
			int res = mActionManager.addAction(action);

			complateAction();
		}

		public override EEvents getActionEvent() {
			return EEvents.eRefreshPosition;
		}

		public override void doUpdateActionParam(Hashtable aData) {}
	}

}