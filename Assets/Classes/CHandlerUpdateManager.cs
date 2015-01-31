using UnityEngine;
using System.Collections;

public delegate void UpdatingEventHandler();

public class CHandlerUpdateManager {
	public event UpdatingEventHandler mUpdatingEvent;

	static CHandlerUpdateManager mInst = null;

	public static CHandlerUpdateManager shared() {
		if (mInst == null) {
			mInst = new CHandlerUpdateManager();
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
