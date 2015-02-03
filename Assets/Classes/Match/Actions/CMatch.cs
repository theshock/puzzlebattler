using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match.Actions {
	public class CMatch : CBase {
		private List<int> mMatchIcons;

		private int mStartDestroyIcon;

		private EIconType mCreateIconType;

		public static IAction create() {
			return new CMatch();
		}

		public EIconType getMatchIconType() {
			CIcon icon = mIconField.GetIconByIndex(mMatchIcons[0]);
			return icon.IconType;
		}

		public void onEndAction(IAction aAction) {
		}

		public override bool validation() {
			if (mMatchIcons != null) {
				foreach (int index_icon in mMatchIcons) {
					CIcon icon = mIconField.GetIconByIndex(index_icon);

					if (!icon || (icon && !icon.IsActionReady())) {
						return false;
					}
				}
			} else {
				return false;
			}

			return true;
		}

		public int getCountMatchIcon() {
			return mMatchIcons.Count;
		}

		public override void startAction() {
			mStartDestroyIcon = 0;

			foreach (int index_icon in mMatchIcons) {
				CIcon icon = mIconField.GetIconByIndex(index_icon);

				var destroy_action = mActionManager.createAction(EAction.eDestroy) as Actions.CDestroy;
				destroy_action.configure(icon);
				destroy_action.setDelegate(onEndAction);

				mStartDestroyIcon += mActionManager.addAction(destroy_action);
			}

			complateAction();
		}


		public override EEvents getActionEvent() {
			return EEvents.eMatch;
		}

		public void configure (List<int> aMatchIcons) {
			mMatchIcons = aMatchIcons;
		}
	}
}