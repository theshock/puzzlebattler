using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match;
using Match.Gem;

namespace Match.Actions {
	public class CSwipe : CBase, IMoveObserver {
		private bool mIsReverseSwipe = false;

		public CIcon mSelectedIcon = null;
		public CIcon mTargetedIcon = null;
		public CPlayer mOwner = null;

		public static IAction create() {
			return new CSwipe();
		}

		public override void ComplateAction() {
			base.ComplateAction();
		}

		public override bool validation() {
			return mOwner != null
				&& mSelectedIcon != null
				&& mTargetedIcon != null
				&& mSelectedIcon.IsActionReady()
				&& mTargetedIcon.IsActionReady()
				&& mOwner.active
				&& mOwner.matches > 0;
		}

		public override void startAction() {
			mOwner.matches--;
			SwipeAnimation();
		}

		public void OnMoveEnd () {
			if (mIsReverseSwipe || IsCorrectSwipe()) {
				ComplateAction();
			} else {
				mOwner.matches++;
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
					mActionManager.AddAction(matchAction);

					return true;
				}
			}

			return false;
		}

		private void SwipeAnimation() {
			mIconField.SwipeIcons(mSelectedIcon, mTargetedIcon).SetObserver(this);
		}

		public override EEvents getActionEvent() {
			return EEvents.eSwipe;
		}
	}
}