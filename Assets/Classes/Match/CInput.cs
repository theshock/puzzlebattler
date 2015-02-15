using Match.Gem;
using UnityEngine;

namespace Match {
	public class CInput : Libraries.IInputObserver {
		public CMatch mController = null;

		protected Libraries.CInput mInput;
		protected CIcon mSelectedIcon = null;

		public CInput (CMatch controller) {
			mController = controller;

			mInput = new Libraries.CInput();
			mInput.registerObserver(this, 0);
		}

		public void Update() {
			mInput.Check();
		}

		public bool OnInputBegin (Vector2 aPosition) {
			var selectedIcon = mController.mView.mField.GetIconByPosition(aPosition);

			if (IsActiveIcon(selectedIcon)) {
				mSelectedIcon = selectedIcon;
				return true;
			} else {
				return false;
			}
		}

		public void OnInputMove (Vector2 aPosition) {
			if (!mController.mGame.mModel.player.CanSwipe()) return;
			if (!IsActiveIcon(mSelectedIcon)) return;

			CIcon targetIcon = mController.mView.mField.GetIconByPosition(aPosition);

			if (mSelectedIcon != targetIcon && IsActiveIcon(targetIcon)) {
				if (mSelectedIcon.IsNeighbour(targetIcon)) {
					if (StartSwipe(mSelectedIcon, targetIcon)) {
						mInput.Block();
					}
				}

				mSelectedIcon = null;
			}
		}

		public void OnInputEnd (Vector2 aPosition) {
			mSelectedIcon = null;
		}

		public bool StartSwipe(CIcon aSelected, CIcon aTargeted) {
			var config = new Actions.CSwipe.Config(){
				iconField = mController.mView.mField,
				selectedIcon = aSelected,
				targetedIcon = aTargeted,
				matcher = mController.mMatcher
			};

			return mController.mActionManager.AddAction(new Actions.CSwipe(config));
		}

		private bool IsActiveIcon (CIcon icon) {
			return icon != null && icon.IsMoveReady() && icon.IsInside();
		}
	}
}