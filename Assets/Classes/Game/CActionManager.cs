using Libraries;
using Match.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game {
	public class CActionManager {
		private ArrayList mActiveActions = new ArrayList();

		public Match.CMatch mMatchController { get; set; }

		public CActionManager() {
			CObjectFactory.registerCreator("match_action_" + (int)Match.EAction.Destroy, CDestroy.create);
			CObjectFactory.registerCreator("match_action_" + (int)Match.EAction.Swipe, CSwipe.create);
			CObjectFactory.registerCreator("match_action_" + (int)Match.EAction.Match, CMatch.create);
			CObjectFactory.registerCreator("match_action_" + (int)Match.EAction.AutoMatch, CAutoMatch.create);
			CObjectFactory.registerCreator("match_action_" + (int)Match.EAction.RefreshPosition, CRefreshPosition.create);
		}

		public IAction createAction(Match.EAction aAction) {
			string name_action = "match_action_" + (int) aAction;

			IAction action = CObjectFactory.createObjectByKey(name_action) as IAction;

			action.SetActionManager(this, mMatchController.mView.mField);

			return action;
		}

		public bool AddAction(IAction aAction) {
			if (aAction.Validation()) {
				aAction.StartAction();
				mActiveActions.Add(aAction);
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