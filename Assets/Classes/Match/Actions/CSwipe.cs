using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match.Actions {
	public class CSwipe : CBase {
		private bool mIsReverseSwipe = false;

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
				&& mSelectedIcon.IsActionReady()
				&& mTargetedIcon.IsActionReady();
		}

		public override void startAction() {
			swipeAnimation();
		}

		public void OnEndSwipeAnimation() {
			mIconField.swipeCellInMatrix(mSelectedIcon, mTargetedIcon);

			// we should open icons before exit of before searching matches
			mSelectedIcon.IconState = EIconState.eOpen;
			mTargetedIcon.IconState = EIconState.eOpen;

			if (mIsReverseSwipe || IsCorrectSwipe()) {
				complateAction();
			} else {
				mIsReverseSwipe = true;
				swipeAnimation();
			}
		}

		private bool IsCorrectSwipe () {
			List<List<int>> matches = mActionManager.mMatchController.mSearcher.FindMatches();

			if (matches.Count > 0) {
				bool swipeCreateMatch = false;

				foreach (List<int> match in matches) {
					if (TryLaunchMatch(match)) {
						swipeCreateMatch = true;
					}
				}

				return swipeCreateMatch;
			} else {
				return false;
			}
		}

		// why should we check for icon contains ?
		private bool TryLaunchMatch (List<int> aMatch) {
			for (int j = 0; j < aMatch.Count; j++) {
				if (aMatch[j] == mSelectedIcon.mIndex || aMatch[j] == mTargetedIcon.mIndex) {
					var matchAction = mActionManager.createAction(EAction.eMatch) as Actions.CMatch;

					matchAction.configure(aMatch);
					mActionManager.addAction(matchAction);

					return true;
				}
			}

			return false;
		}

		private void swipeAnimation() {
			Vector3 objSFPoint = mSelectedIcon.transform.position;
			Vector3 objSWPoint = mTargetedIcon.transform.position;

			mSelectedIcon.IconState = EIconState.eLock;
			mTargetedIcon.IconState = EIconState.eLock;

			mTargetedIcon.mDelegate = OnEndSwipeAnimation;
			mSelectedIcon.mDelegate = OnEndSwipeAnimation;

			float duration = 0.2f;

			iTween.MoveTo(mTargetedIcon.gameObject, iTween.Hash("transition", "easeInOut", "position", objSFPoint, "time", duration));
			iTween.MoveTo(mSelectedIcon.gameObject, iTween.Hash("transition", "easeInOut", "position", objSWPoint, "time", duration, "oncomplete", "onEndSwipeAnimation"));
		}

		public override EEvents getActionEvent() {
			return EEvents.eSwipe;
		}

		public void configure (CIcon selected, CIcon targeted) {
			mSelectedIcon = selected;
			mTargetedIcon = targeted;
		}
	}
}