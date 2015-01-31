using Library;
using System.Collections;
using UnityEngine;

namespace Match.Actions {
	public class CDestroy : CBase {
		private float mTimeDelayEmpty = 0.1f;
		private CIcon mIconTarget = null;

		public static IAction create() {
			return new CDestroy();
		}

		public override bool validation() {
			return mIconTarget && mIconTarget.getIsReadyAction();
		}

		public override void startAction() {
			mIconTarget.IconState = EIconState.eLock;
			mIconTarget.onDestroyIcon();

			CGlobalUpdateManager.shared().subscribeUpdateEvent(doUpdate);
		}

		public void emptyCell() {
			mIconTarget.IconState = EIconState.eClear;
			complateAction();
		}

		public void doUpdate() {
			mTimeDelayEmpty -= Time.smoothDeltaTime;

			if (mTimeDelayEmpty <= 0) {
				CGlobalUpdateManager.shared().unsubscribeUpdateEvent(doUpdate);
				emptyCell();
			}
		}


		public override EEvents getActionEvent() {
			return  EEvents.eActionDestroy;
		}

		public override void doUpdateActionParam(Hashtable aData) {
			mIconTarget = aData["target"] as CIcon;
		}

		public void destroyArray(ArrayList aCells) {}
	}

}