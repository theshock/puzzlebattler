using UnityEngine;
using System.Collections;

namespace Match.Actions {
	public class CDestroy : CBase {
		private float mTimeDelayEmpty = 0.1f;
		private CIcon mIconTarget = null;

		public static IAction create() {
			return new CDestroy();
		}

		public override bool validation() {
			//		Debug.Log("CMatchActionDestroy validation");
			//		Debug.Log("CMatchActionDestroy validation mIconTarget State" + mIconTarget.IconState);

			if (!mIconTarget) {
				//			Debug.Log("CMatchActionDestroy validation fail 1");

				return false;
			}

			if (!mIconTarget.getIsReadyAction()) {
				//			Debug.Log("CMatchActionDestroy validation fail 2");

				return false;
			}

			//		Debug.Log("CMatchActionDestroy validation ok");

			return true;
		}

		public override void startAction() {
			//		Debug.Log("CMatchActionDestroy startAction");

			mIconTarget.IconState = EIconState.eLock;
			mIconTarget.onDestroyIcon();

			//		Debug.Log("CMatchActionDestroy startAction 1");

			CHandlerUpdateManager.shared().subscribeUpdateEvent(doUpdate);
		}

		public void emptyCell() {
			//		Debug.Log("CMatchActionDestroy emptyCell");

			mIconTarget.IconState = EIconState.eClear;
			complateAction();
		}

		public void doUpdate() {
			mTimeDelayEmpty -= Time.smoothDeltaTime;
			//		Debug.Log("CMatchActionDestroy doUpdate mTimeDelayEmpty = " + mTimeDelayEmpty);

			if (mTimeDelayEmpty <= 0) {
				CHandlerUpdateManager.shared().unsubscribeUpdateEvent(doUpdate);
				emptyCell();
			}
		}


		public override GameNotificationEvents getActionEvent() {
			return  GameNotificationEvents.eMatchActionDestroyEvent;
		}

		public override void doUpdateActionParam(Hashtable aData) {
			mIconTarget = aData["target"] as CIcon;

			return;
		}

		public void destroyArray(ArrayList aCells) {

		}
	}

}