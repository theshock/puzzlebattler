using Match.Gem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match.Actions {
	public class CMatch : CBase {
		private List<CIcon> mMatchIcons;

		public CMatch (List<CIcon> aMatchIcons) {
			mMatchIcons = aMatchIcons;
		}

		public EType GetMatchIconType() {
			return mMatchIcons[0].IconType;
		}

		public override void OnActionEnd (IAction aAction) {}

		public override bool Validation() {
			if (mMatchIcons == null) {
				return false;
			}

			foreach (CIcon icon in mMatchIcons) {
				if (icon == null || !icon.IsActionReady()) {
					return false;
				}
			}

			return true;
		}

		public int GetCountMatchIcon() {
			return mMatchIcons.Count;
		}

		public override void StartAction() {
			foreach (CIcon icon in mMatchIcons) {
				var destroyAction = new CDestroy(icon);
				destroyAction.SetObserver(this);

				mActionManager.AddAction(destroyAction);
			}

			// todo: add score count
			// mIconField.mMatch.AddScore(GetCountMatchIcon());

			ComplateAction();
		}


		public override EEvents GetActionEvent() {
			return EEvents.Match;
		}
	}
}