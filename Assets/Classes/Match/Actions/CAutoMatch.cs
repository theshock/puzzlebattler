using Match.Gem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match.Actions {

	public class CAutoMatch : CBase, IObserver {
		private List<List<CIcon>> mAutoMatches;

		public static IAction create() {
			return new CAutoMatch();
		}

		public void OnActionEnd(IAction aAction) {}

		public override bool Validation() {
			return mAutoMatches != null && mAutoMatches.Count > 0;
		}

		public override void StartAction() {
			foreach (List<CIcon> match in mAutoMatches) {
				CMatch action = mActionManager.createAction(EAction.Match) as CMatch;
				action.Configure(match);
				action.SetObserver(this);
				mActionManager.AddAction(action);
			}

			ComplateAction();
		}

		public void doUpdate() {}

		public override EEvents GetActionEvent() {
			return EEvents.AutoMatch;
		}

		public void autoConfigure () {
			Configure(mActionManager.mMatchController.mSearcher.FindMatches());
		}

		public void Configure(List<List<CIcon>> aMatches) {
			mAutoMatches = aMatches;
		}
	}

}

