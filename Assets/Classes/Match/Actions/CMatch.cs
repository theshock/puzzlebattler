using UnityEngine;
using System.Collections;

namespace Match.Actions {
	public class CMatch : CBase {
		private bool mIsHorizontalSwipe = true;

		private ArrayList mMatchIcons = null;

		private int mStartDestroyIcon;

		private EIconType mCreateIconType;

		public static IAction create() {
			return new CMatch();
		}

		public EIconType getMatchIconType() {
			CIcon icon = mIconField.getIconByIndex((int)mMatchIcons[0]);
			return icon.IconType;
		}

		public void onEndAction(IAction aAction) {
			//		Debug.Log("CMatchActionMatch onEndAction");
			//
			//		mStartDestroyIcon--;
			//
			//		if(mStartDestroyIcon == 0)
			//		{
			//			Debug.Log("CMatchActionMatch onEndAction last");
			//
			//			complateAction();
			//		}
		}

		public override bool validation() {
			//		Debug.Log("CMatchActionMatch validation");

			if (mMatchIcons != null) {
				foreach (int index_icon in mMatchIcons) {
					CIcon icon = mIconField.getIconByIndex(index_icon);

					if (!icon || (icon && !icon.getIsReadyAction())) {
						return false;
					}
				}
			} else {
				return false;
			}

			//		Debug.Log("CMatchActionMatch validation ok");

			return true;
		}

		public int getCountMatchIcon() {
			return mMatchIcons.Count;
		}

		public override void startAction() {
			//		Debug.Log("CMatchActionMatch startAction");

			mStartDestroyIcon = 0;

			foreach (int index_icon in mMatchIcons) {
				CIcon icon = mIconField.getIconByIndex(index_icon);

				IAction destroy_action = mActionManager.createAction(EMatchAction.eDestroyAction);
				Hashtable hash = new Hashtable();
				hash.Add("target", icon);
				destroy_action.doUpdateActionParam(hash);
				destroy_action.setDelegate(onEndAction);

				mStartDestroyIcon += mActionManager.addAction(destroy_action);
			}

			complateAction();
			//		Debug.Log("CMatchActionMatch startAction end " + mStartDestroyIcon);
		}


		public override GameNotificationEvents getActionEvent() {
			return GameNotificationEvents.eMatchActionMatchEvent;
		}

		public override void doUpdateActionParam(Hashtable aData) {
			mIsHorizontalSwipe = (bool) aData["directionSwap"];
			mMatchIcons = aData["matchIcons"] as ArrayList;

			return;
		}
	}
}