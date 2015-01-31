using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Match.Actions {
	public class CBase : Object, IAction {
		protected CGameActionManager mActionManager;
		protected Match.Actions.Delegate mDelegate;
		protected CField mIconField;

		public void initWithActionManager(CGameActionManager aManager, CField aIconField) {
			mActionManager = aManager;
			mIconField = aIconField;
		}

		public void setDelegate(Match.Actions.Delegate aDelegate) {
			mDelegate = aDelegate;
		}

		public virtual void doUpdateActionParam(Hashtable aData) {
			return;
		}

		public virtual bool validation() {
			return false;
		}

		public virtual void startAction() {

		}

		public virtual void complateAction() {
			try {
				mActionManager.mMatchController.mNotificationManager.notify((int)getActionEvent(), this);

				if (mDelegate != null) {
					mDelegate.Invoke(this);
				}

				mActionManager.onEndAction(this);

			}
			catch (System.Exception e) {
				Debug.LogError(e.ToString());
			}
		}

		public virtual GameNotificationEvents getActionEvent() {
			return GameNotificationEvents.eGameNotificationEventsCount;
		}
	}

}