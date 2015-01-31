using UnityEngine;
using System.Collections;

namespace Match.Actions {
	public class CSwipe : CBase {
		private bool mIsCurrentSwipe = true;
		private bool mIsCurrenSwipe = false;

		private CIcon mSelectedIcon = null;
		private CIcon mTargetedIcon = null;

		public static IAction create() {
			return new CSwipe();
		}

		public override void complateAction() {
			mSelectedIcon.mDelegate = null;
			mTargetedIcon.mDelegate = null;

			base.complateAction();
		}

		public override bool validation() {
			return mSelectedIcon != null
				&& mTargetedIcon != null
				&& mSelectedIcon.getIsReadyAction()
				&& mTargetedIcon.getIsReadyAction();
		}

		public void onEndSwipeAnimation() {
			mIconField.swipeCellInMatrix(mSelectedIcon, mTargetedIcon);

			// Открываем ячейки
			mSelectedIcon.IconState = EIconState.eOpen;
			mTargetedIcon.IconState = EIconState.eOpen;

			if (mIsCurrenSwipe == false) {
				bool swipeCreateMatch = false;

				ArrayList matches = mActionManager.mMatchController.mSearcher.findMatches(true);

				if (matches.Count > 0) {
					foreach (ArrayList match in matches) {
						for (int j = 0; j < match.Count; j++) {

							if (((int)match[j] == mSelectedIcon.mIndex) || ((int)match[j] == mTargetedIcon.mIndex)) {
								IAction matchAction = mActionManager.createAction(EAction.eMatch);
								bool is_hor = (mSelectedIcon.mRow == mTargetedIcon.mRow);

								Hashtable hash = new Hashtable();
								hash["directionSwap"] = is_hor;
								hash["matchIcons"] = match;
								matchAction.doUpdateActionParam(hash);

								mActionManager.addAction(matchAction);

								swipeCreateMatch = true;
							}
						}

					}

					if (swipeCreateMatch) {
						complateAction();
					} else {
						mSelectedIcon.IconState = EIconState.eLock;
						mTargetedIcon.IconState = EIconState.eLock;

						mIsCurrenSwipe = true;

						swipeAnimation(true);
					}

				} else {
					// Открываем ячейки
					mSelectedIcon.IconState = EIconState.eOpen;
					mTargetedIcon.IconState = EIconState.eOpen;

					if (swipeCreateMatch) {
						complateAction();
					} else {
						mSelectedIcon.IconState = EIconState.eLock;
						mTargetedIcon.IconState = EIconState.eLock;

						mIsCurrenSwipe = true;

						swipeAnimation(true);
					}
				}
			} else {
				complateAction();
			}
		}

		private void swipeAnimation(bool aIsWrongSwipe) {
			Vector3 objSFPoint = mSelectedIcon.transform.position;
			Vector3 objSWPoint = mTargetedIcon.transform.position;
			mSelectedIcon.IconState = EIconState.eLock;
			mTargetedIcon.IconState = EIconState.eLock;

			mTargetedIcon.mDelegate = onEndSwipeAnimation;
			mSelectedIcon.mDelegate = onEndSwipeAnimation;

			if (!aIsWrongSwipe) {
				float duration = 0.2f;

				iTween.MoveTo(mTargetedIcon.gameObject, iTween.Hash("transition", "easeInOut", "position", objSFPoint, "time", duration));
				iTween.MoveTo(mSelectedIcon.gameObject, iTween.Hash("transition", "easeInOut", "position", objSWPoint, "time", duration, "onComplete", "onEndSwipeAnimation"));

			} else {
				float duration = 0.2f;

				iTween.MoveTo(mTargetedIcon.gameObject, iTween.Hash("transition", "easeInOut", "position", objSFPoint, "time", duration));
				iTween.MoveTo(mSelectedIcon.gameObject, iTween.Hash("transition", "easeInOut", "position", objSWPoint, "time", duration, "onComplete", "onEndSwipeAnimation"));
			}
		}

		public override void startAction() {
			mIsCurrentSwipe = false;

			swipeAnimation(mIsCurrentSwipe);
		}


		public override EEvents getActionEvent() {
			return  EEvents.eActionSwipe;
		}

		public override void doUpdateActionParam(Hashtable aData) {
			mSelectedIcon = aData["selectedIcon"] as CIcon;
			mTargetedIcon = aData["targetedIcon"] as CIcon;

			return;
		}
	}
}