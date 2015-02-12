using Match.Gem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match.Actions {

	public class CAutoMatch : CBase {
		private List<List<CIcon>> mAutoMatches;

		public CAutoMatch (List<List<CIcon>> aMatches) {
			mAutoMatches = aMatches;
		}

		public override void OnActionEnd(IAction aAction) {}

		public override bool Validation() {
			return mAutoMatches != null && mAutoMatches.Count > 0;
		}

		public override void StartAction() {
			foreach (List<CIcon> match in mAutoMatches) {
				CMatch action = new CMatch(match);
				action.SetObserver(this);
				mActionManager.AddAction(action);
			}

			ComplateAction();
		}

		public override EEvents GetActionEvent() {
			return EEvents.AutoMatch;
		}

	}

}

