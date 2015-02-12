using Libraries;
using Match.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game {
	public class CActionManager {
		private ArrayList mActiveActions = new ArrayList();

		public Match.CMatch mMatchController { get; set; }

		public bool AddAction(IAction aAction) {
			aAction.SetActionManager(this);

			if (aAction.Validation()) {
				mActiveActions.Add(aAction);
				aAction.StartAction();
				return true;
			} else {
				return false;
			}
		}

		public void removeAction(IAction aAction) {
			try {
				mActiveActions.Remove(aAction);
			} catch (System.Exception e) {
				Debug.LogError(e.ToString());
			}
		}

		public void OnActionEnd(CBase aAction) {
			mMatchController.mNotificationManager.notify((int)aAction.GetActionEvent(), aAction);

			removeAction(aAction);

			if (!HasActions()) {
				mMatchController.mNotificationManager.notify((int)Match.EEvents.Finish, null);
			}
		}

		public bool HasActions() {
			return (mActiveActions.Count > 0);
		}
	}
}