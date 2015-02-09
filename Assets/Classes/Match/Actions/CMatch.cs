using Match.Gem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match.Actions {
	public class CMatch : CBase {
		private List<int> mMatchIcons;

		private int mStartDestroyIcon;

		private EType mCreateIconType;

		public static IAction create() {
			return new CMatch();
		}

		public EType getMatchIconType() {
			return mIconField.GetIconByIndex(mMatchIcons[0]).IconType;
		}

		public void onEndAction(IAction aAction) {
		}

		public override bool validation() {
			if (mMatchIcons == null) {
				return false;
			}

			foreach (int index in mMatchIcons) {
				CIcon icon = mIconField.GetIconByIndex(index);

				if (icon == null || !icon.IsActionReady()) {
					return false;
				}
			}

			return true;
		}

		public int getCountMatchIcon() {
			return mMatchIcons.Count;
		}

		public override void startAction() {
			mStartDestroyIcon = 0;

			foreach (int index in mMatchIcons) {
				CIcon icon = mIconField.GetIconByIndex(index);

				var destroyAction = mActionManager.createAction(EAction.eDestroy) as Actions.CDestroy;
				destroyAction.configure(icon);
				destroyAction.setDelegate(onEndAction);

				mStartDestroyIcon += mActionManager.AddAction(destroyAction) ? 1 : 0;
			}

			ComplateAction();
		}


		public override EEvents getActionEvent() {
			return EEvents.eMatch;
		}

		public void configure (List<int> aMatchIcons) {
			mMatchIcons = aMatchIcons;
		}
	}
}