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
			SwipeAnimation();
		}

		public void OnEndSwipeAnimation() {
			mIconField.mOnIconsMoveEnd -= OnEndSwipeAnimation;

			// we should open icons before exit of before searching matches
			mSelectedIcon.IconState = EIconState.eOpen;
			mTargetedIcon.IconState = EIconState.eOpen;

			if (mIsReverseSwipe || IsCorrectSwipe()) {
				complateAction();
			} else {
				mIsReverseSwipe = true;
				SwipeAnimation();
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

		private void SwipeAnimation() {
			mIconField.mOnIconsMoveEnd += OnEndSwipeAnimation;
			mIconField.SwipeIcons(mSelectedIcon, mTargetedIcon);
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