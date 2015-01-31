using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match.Actions {

	public class CAutoMatch : CBase {
		private List<List<int>> mAutoMatches;
		public int mCountStartMatch = 0;

		public static IAction create() {
			return new CAutoMatch();
		}

		public void onEndAction(IAction aAction) {	}

		public override bool validation() {
			mAutoMatches = mActionManager.mMatchController.mSearcher.findMatches();

			return mAutoMatches.Count > 0;
		}

		public override void startAction() {
			mCountStartMatch = 0;

			foreach (List<int> match in mAutoMatches) {
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
			return EEvents.eAutoMatch;
		}

		public override void doUpdateActionParam(Hashtable aData) {
			mAutoMatches = aData["matches"] as List<List<int>>;
		}

	}

}

