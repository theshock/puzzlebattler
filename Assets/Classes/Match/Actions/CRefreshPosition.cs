using UnityEngine;
using System.Collections;

namespace Match.Actions {
	public class CRefreshPosition : CBase {
		private int mCountStartUpdateIcon = 0;

		public static IAction create() {
			return new CRefreshPosition();
		}


		public override bool validation() {
			//		Debug.Log("CMatchRefreshPositionAction validation");

			if (mIconField.isHaveEmptyIcon()) {
				//			Debug.Log("CMatchRefreshPositionAction validation ok");

				return true;
			}

			//		Debug.Log("CMatchRefreshPositionAction validation fail");

			return false;
		}

		public override void startAction() {
			mCountStartUpdateIcon = mIconField.updatePositionIcons(onEndMove);

			if (mCountStartUpdateIcon == 0) {
				IAction action = mActionManager.createAction(EMatchAction.eAutoMatchAction);
				mActionManager.addAction(action);

				complateAction();
			}

		}

		public void doUpdate() {

		}

		public void onEndMove() {
			IAction action = mActionManager.createAction(EMatchAction.eAutoMatchAction);
			int res = mActionManager.addAction(action);

			complateAction();
		}

		public override GameNotificationEvents getActionEvent() {
			return GameNotificationEvents.eMatchRefreshPositionEvent;
		}

		public override void doUpdateActionParam(Hashtable aData) {

			return;
		}
	}

}