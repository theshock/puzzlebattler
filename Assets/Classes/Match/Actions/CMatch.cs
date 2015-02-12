using Match.Gem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match.Actions {
	public class CMatch : CBase, IObserver {
		private List<CIcon> mMatchIcons;

		public static IAction create() {
			return new CMatch();
		}

		public EType GetMatchIconType() {
			return mMatchIcons[0].IconType;
		}

		public void OnActionEnd (IAction aAction) {}

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
				var destroyAction = mActionManager.createAction(EAction.Destroy) as Actions.CDestroy;
				destroyAction.configure(icon);
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

		public void Configure(List<CIcon> aMatchIcons) {
			mMatchIcons = aMatchIcons;
		}
	}
}