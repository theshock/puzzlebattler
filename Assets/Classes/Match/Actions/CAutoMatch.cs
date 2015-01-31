using UnityEngine;
using System.Collections;

namespace Match.Actions {

	public class CAutoMatch : CBase {
		private ArrayList mAutoMatches = null;
		public int mCountStartMatch = 0;

		public static IAction create() {
			return new CAutoMatch();
		}

		public void onEndAction(IAction aAction) {	}

		public override bool validation() {
			mAutoMatches = mActionManager.mMatchController.mSearcher.findMatches(true);

			return mAutoMatches.Count > 0;
		}

		public override void startAction() {
			mCountStartMatch = 0;

			foreach (ArrayList match in mAutoMatches) {
				bool is_hor = true;

				CMatch action = mActionManager.createAction(EAction.eMatch) as CMatch;

				Hashtable hash = new Hashtable();
				hash["matchIcons"] = match;
				hash["directionSwap"] = is_hor;

				action.doUpdateActionParam(hash);
				action.setDelegate(onEndAction);

				mCountStartMatch += mActionManager.addAction(action);
			}

			complateAction();
		}


		public void doUpdate() {}


		public override EEvents getActionEvent() {
			return EEvents.eActionAutoMatch;
		}

		public override void doUpdateActionParam(Hashtable aData) {
			mAutoMatches = aData["matches"] as ArrayList;
		}

	}

}

