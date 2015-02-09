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

		public void onEndAction(IAction aAction) {}

		public override bool validation() {
			return mAutoMatches != null && mAutoMatches.Count > 0;
		}

		public override void startAction() {
			mCountStartMatch = 0;

			foreach (List<int> match in mAutoMatches) {
				CMatch action = mActionManager.createAction(EAction.eMatch) as CMatch;
				action.configure(match);
				action.setDelegate(onEndAction);

				mCountStartMatch += mActionManager.AddAction(action) ? 1 : 0;
			}

			ComplateAction();
		}


		public void doUpdate() {}


		public override EEvents getActionEvent() {
			return EEvents.eAutoMatch;
		}

		public void autoConfigure () {
			configure(mActionManager.mMatchController.mSearcher.FindMatches());
		}

		public void configure (List<List<int>> aMatches) {
			mAutoMatches = aMatches;
		}
	}

}

