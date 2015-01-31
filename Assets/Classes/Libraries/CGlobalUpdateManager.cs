using UnityEngine;
using System.Collections;

namespace Libraries {
	public delegate void UpdatingEventHandler();

	public class CGlobalUpdateManager {
		public event UpdatingEventHandler mUpdatingEvent;

		static CGlobalUpdateManager mInst = null;

		public static CGlobalUpdateManager shared() {
			if (mInst == null) {
				mInst = new CGlobalUpdateManager();
			}

			return mInst;
		}

		public void doUpdate() {
			if (mUpdatingEvent != null) {
				mUpdatingEvent();
			}
		}

		public void unsubscribeUpdateEvent(UpdatingEventHandler aDelegate) {
			mUpdatingEvent -= aDelegate;
		}

		public void subscribeUpdateEvent(UpdatingEventHandler aDelegate) {
			mUpdatingEvent += aDelegate;
		}
	}
}