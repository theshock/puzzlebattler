using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Match.Actions {
	public class CBase : Object, IAction {
		protected Game.CActionManager mActionManager;
		protected Match.Actions.Delegate mDelegate;
		protected CField mIconField;

		public void initWithActionManager(Game.CActionManager aManager, CField aIconField) {
			mActionManager = aManager;
			mIconField = aIconField;
		}

		public void setDelegate(Match.Actions.Delegate aDelegate) {
			mDelegate = aDelegate;
		}

		public virtual bool validation() {
			return false;
		}

		public virtual void startAction() {

		}

		public virtual void ComplateAction() {
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

		public virtual EEvents getActionEvent() {
			return EEvents.Count;
		}
	}

}