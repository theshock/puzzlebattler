using Match.Gem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match {

	public class CView : MonoBehaviour, IInputObserver {
		public CField mField = null;
		public CMatch mController = null;

		private CInput mInput;
		private CIcon mSelectedIcon = null;

		void Start() {
			mInput = new CInput();
			mInput.registerObserver(this, 0);
		}

		public void init() {
			mField.InitMatchField();
		}

		public void Update() {
			mInput.Check();
		}

		public void DestroyRow(int aRow) {
			DestroyList(mField.GetIconsByRow(aRow));
		}

		public void DestroyColumn(int aCol) {
			DestroyList(mField.GetIconsByColumn(aCol));
		}

		public bool OnInputBegin (Vector2 aPosition) {
			mSelectedIcon = mField.GetIconByPosition(aPosition);

			return mSelectedIcon != null && mSelectedIcon.IsMoveReady();
		}

		public void OnInputMove (Vector2 aPosition) {
			if (!IsActiveIcon(mSelectedIcon)) return;

			CIcon targetIcon = mField.GetIconByPosition(aPosition);

			if (IsActiveIcon(targetIcon) && mSelectedIcon != targetIcon) {
				if (mSelectedIcon.IsNeighbour(targetIcon)) {
					StartSwipe(mSelectedIcon, targetIcon, mController.player);
				}

				mSelectedIcon = null;
			}
		}

		public void AiSwipe (CIcon aSelected, CIcon aTargeted) {
			StartSwipe(aSelected, aTargeted, mController.opponent);
		}

		public void OnInputEnd (Vector2 aPosition) {
			mSelectedIcon = null;
		}

		private void DestroyList (List<CIcon> aIcons) {
			foreach (CIcon icon in aIcons) {
				var destroyAction = new Actions.CDestroy(icon);
				mController.mActionManager.AddAction(destroyAction);
			}
		}

		private void StartSwipe(CIcon aSelected, CIcon aTargeted, CPlayer owner) {
			var swipeAction = new Actions.CSwipe();

			swipeAction.mIconField = mField;
			swipeAction.mSelectedIcon = aSelected;
			swipeAction.mTargetedIcon = aTargeted;
			swipeAction.mOwner = owner;

			if (mController.mActionManager.AddAction(swipeAction)) {
				mInput.Block();
			}
		}

		private bool IsActiveIcon (CIcon icon) {
			return icon != null && icon.IsMoveReady() && icon.IsInside();
		}
	}

}