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
			CObjectFactory.registerCreator("match_action_" + (int)Match.EAction.eDestroy, CDestroy.create);
			CObjectFactory.registerCreator("match_action_" + (int)Match.EAction.eSwipe, CSwipe.create);
			CObjectFactory.registerCreator("match_action_" + (int)Match.EAction.eMatch, CMatch.create);
			CObjectFactory.registerCreator("match_action_" + (int)Match.EAction.eAutoMatch, CAutoMatch.create);
			CObjectFactory.registerCreator("match_action_" + (int)Match.EAction.eRefreshPosition, CRefreshPosition.create);
		}

		public IAction createAction(Match.EAction aAction) {
			string name_action = "match_action_" + (int) aAction;

			IAction action = CObjectFactory.createObjectByKey(name_action) as IAction;

			action.initWithActionManager(this, mMatchController.mView.mField);

			return action;
		}

		public bool AddAction(IAction aAction) {
			if (aAction.validation()) {
				aAction.startAction();
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

		public void onEndAction(IAction aAction) {
			removeAction(aAction);

			if (!HasActions()) {
				mMatchController.mNotificationManager.notify((int)Match.EEvents.eEndAll, null);
			}
		}

		public bool HasActions() {
			return (mActiveActions.Count > 0);
		}
	}
}