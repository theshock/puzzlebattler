using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Match.Actions {
	public abstract class CBase : UnityEngine.Object, IAction {

		protected IObserver mObserver;
		protected Game.CActionManager mActionManager;

		public void SetObserver (IObserver observer) {
			this.mObserver = observer;
		}

		public void SetActionManager(Game.CActionManager aManager, CField aIconField) {
			this.mActionManager = aManager;
		}

		public abstract Match.EEvents GetActionEvent();
		public abstract bool Validation ();
		public abstract void StartAction ();

		public void ComplateAction() {
			mActionManager.mMatchController.mNotificationManager.notify((int)GetActionEvent(), this);

			mActionManager.OnActionEnd(this);

			if (mObserver != null) {
				mObserver.OnActionEnd(this);
			}
		}
	}

}