using Match.Gem;
using UnityEngine;

namespace Match {
	public class CInput : Libraries.IInputObserver {
		public CField field = null;

		protected Libraries.CInput mInput;
		protected CIcon mSelectedIcon = null;

		public CInput (CField field) {
			this.field = field;

			mInput = new Libraries.CInput();
			mInput.registerObserver(this, 0);
		}

		public void Update() {
			mInput.Check();
		}

		public bool OnInputBegin (Vector2 aPosition) {
			var selectedIcon = field.GetIconByPoint(aPosition);

			if (IsActiveIcon(selectedIcon)) {
				mSelectedIcon = selectedIcon;
				return true;
			} else {
				return false;
			}
		}

		public void OnInputMove (Vector2 position) {
			if (!Game.Instance.mModel.player.CanSwipe()) return;
			if (!IsActiveIcon(mSelectedIcon)) return;

			CIcon targetIcon = field.GetIconByPoint(position);

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

		public bool StartSwipe(CIcon selected, CIcon targeted) {
			return field.actions.AddAction(new Actions.CSwipe(field, selected, targeted));
		}

		private bool IsActiveIcon (CIcon icon) {
			return icon != null && icon.IsIdle();
		}
	}
}