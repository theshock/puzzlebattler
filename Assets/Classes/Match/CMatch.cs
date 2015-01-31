using Libraries;
using System.Collections;
using UnityEngine;

namespace Match {
	public class CMatch : MonoBehaviour , INotificationObserver {
		public CNotificationManager mNotificationManager = new CNotificationManager();
		public CSearcher mSearcher { get; set; }
		public CView mView;
		public Game.CActionManager mActionManager { get; set; }

		public void handleNotification(int aEvent, Object aParam, CNotificationManager aManager) {
			if ((EEvents) aEvent == EEvents.eEndAll) {
				Actions.IAction action = mActionManager.createAction(EAction.eRefreshPosition);
				mActionManager.addAction(action);
			}
		}

		void Start() {
			mActionManager = new Game.CActionManager();
			mActionManager.mMatchController = this;

			mNotificationManager.addObserver((int)EEvents.eEndAll, this);

			mSearcher = new CSearcher();
			mSearcher.mController = this;

			mView.init();

			Actions.IAction action = mActionManager.createAction(EAction.eAutoMatch);
			mActionManager.addAction(action);
		}

		void Update() {
			CGlobalUpdateManager.shared().doUpdate();
		}
	}
}