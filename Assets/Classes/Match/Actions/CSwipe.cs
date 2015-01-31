using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

				List<List<int>> matches = mActionManager.mMatchController.mSearcher.findMatches();

				if (matches.Count > 0) {
					foreach (List<int> match in matches) {
						for (int j = 0; j < match.Count; j++) {

							if (match[j] == mSelectedIcon.mIndex || match[j] == mTargetedIcon.mIndex) {
								var matchAction = mActionManager.createAction(EAction.eMatch) as Actions.CMatch;
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

			float duration = 0.2f;

			iTween.MoveTo(mTargetedIcon.gameObject, iTween.Hash("transition", "easeInOut", "position", objSFPoint, "time", duration));
			iTween.MoveTo(mSelectedIcon.gameObject, iTween.Hash("transition", "easeInOut", "position", objSWPoint, "time", duration, "onComplete", "onEndSwipeAnimation"));
		}

		public override void startAction() {
			mIsCurrentSwipe = false;

			swipeAnimation(mIsCurrentSwipe);
		}


		public override EEvents getActionEvent() {
			return  EEvents.eSwipe;
		}

		public struct Config {
			public CIcon selected;
			public CIcon targeted;
		}

		public void configure (CIcon selected, CIcon targeted) {
			mSelectedIcon = selected;
			mTargetedIcon = targeted;
		}
	}
}