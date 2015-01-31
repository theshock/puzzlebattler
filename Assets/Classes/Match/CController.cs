using System.Collections;
using UnityEngine;

namespace Match {
	public class CController : MonoBehaviour , INotificationObserver {
		public CNotificationManager mNotificationManager = new CNotificationManager();
		public CSearcher mMatchSearcher { get; set; }
		public CView mMatchView;
		public CGameActionManager mActionManager { get; set; }

		public void handleNotification(int aEvent, Object aParam, CNotificationManager aManager) {
			if ((GameNotificationEvents) aEvent == GameNotificationEvents.eMatchEventEndAllAction) {
				Actions.IAction action = mActionManager.createAction(EMatchAction.eRefreshPositionAction);
				mActionManager.addAction(action);
			}
		}

		void Start() {
			mActionManager = new CGameActionManager();
			mActionManager.mMatchController = this;

			mNotificationManager.addObserver((int)GameNotificationEvents.eMatchEventEndAllAction, this);

			mMatchSearcher = new CSearcher();
			mMatchSearcher.mMatchController = this;
			mMatchSearcher.subscribeNotification(mNotificationManager);

			mMatchView.init();

			Actions.IAction action = mActionManager.createAction(EMatchAction.eAutoMatchAction);
			mActionManager.addAction(action);
		}

		void Update() {
			CHandlerUpdateManager.shared().doUpdate();
		}
	}
}