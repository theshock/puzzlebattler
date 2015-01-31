using UnityEngine;
using System.Collections;

namespace Match.Actions {
	public class CSwipe : CBase {
		private bool mIsCurrentSwipe = true;

		private CIcon mSelectedIcon = null;

		private CIcon mTargetedIcon = null;

		private bool mIsCurrenSwipe = false;

		public static IAction create() {
			return new CSwipe();
		}

		public override void complateAction() {
			mSelectedIcon.mDelegate = null;
			mTargetedIcon.mDelegate = null;

			base.complateAction();
		}

		public override bool validation() {
			if (mSelectedIcon == null || mTargetedIcon == null) {
				return false;
			}


			if (!mSelectedIcon.getIsReadyAction() || !mTargetedIcon.getIsReadyAction()) {
				return false;
			}

			return true;
		}

		public void onEndSwipeAnimation() {
			//		Debug.Log("onEndSwipeAnimation");
			//		Debug.Log("mIsCurrenSwipe = " + mIsCurrenSwipe);
			//
			mIconField.swipeCellInMatrix(mSelectedIcon, mTargetedIcon);

			// Открываем ячейки
			mSelectedIcon.IconState = EIconState.eOpen;
			mTargetedIcon.IconState = EIconState.eOpen;

			if (mIsCurrenSwipe == false) {
				bool swipeCreateMatch = false;

				ArrayList matches = mActionManager.mMatchController.mMatchSearcher.findMatches(true);

				if (matches.Count > 0) {
					foreach (ArrayList match in matches) {
						for (int j = 0; j < match.Count; j++) {

							if (((int)match[j] == mSelectedIcon.mIndex) || ((int)match[j] == mTargetedIcon.mIndex)) {
								IAction matchAction = mActionManager.createAction(EMatchAction.eMatchAction);
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
						//					Debug.Log("onEndSwipeAnimation did not find match");

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
				//			Debug.Log("onEndSwipeAnimation mIsCurrenSwipe true");

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
				//			Debug.Log("swipeAnimation true");

				float duration = 0.2f;

				iTween.MoveTo(mTargetedIcon.gameObject, iTween.Hash("transition", "easeInOut", "position", objSFPoint, "time", duration));
				iTween.MoveTo(mSelectedIcon.gameObject, iTween.Hash("transition", "easeInOut", "position", objSWPoint, "time", duration, "onComplete", "onEndSwipeAnimation"));

				//			Debug.Log("swipeAnimation true 1");
			}
		}

		public override void startAction() {
			mIsCurrentSwipe = false;

			swipeAnimation(mIsCurrentSwipe);
		}


		public override GameNotificationEvents getActionEvent() {
			return  GameNotificationEvents.eMatchActionSwipeEvent;
		}

		public override void doUpdateActionParam(Hashtable aData) {
			mSelectedIcon = aData["selectedIcon"] as CIcon;
			mTargetedIcon = aData["targetedIcon"] as CIcon;

			return;
		}
	}
}