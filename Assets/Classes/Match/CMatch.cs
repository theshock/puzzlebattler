using Libraries;
using System.Collections;
using UnityEngine;

namespace Match {
	public class CMatch : MonoBehaviour , INotificationObserver {
		public CNotificationManager mNotificationManager = new CNotificationManager();
		public CSearcher mSearcher;
		public CView mView;
		public Game.CGame mGame;
		public Game.CActionManager mActionManager;

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

			var action = mActionManager.createAction(EAction.eAutoMatch) as Actions.CAutoMatch;
			action.autoConfigure();
			mActionManager.addAction(action);
		}

		void Update() {
			CGlobalUpdateManager.shared().doUpdate();
		}
	}
}