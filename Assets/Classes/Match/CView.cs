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
					StartSwipe(mSelectedIcon, targetIcon);
				}

				mSelectedIcon = null;
			}
		}

		public void OnInputEnd (Vector2 aPosition) {
			mSelectedIcon = null;
		}

		private void DestroyList (List<CIcon> aIcons) {
			foreach (CIcon icon in aIcons) {
				var destroyAction = mController.mActionManager.createAction(EAction.eDestroy) as Actions.CDestroy;
				destroyAction.configure(icon);
				mController.mActionManager.AddAction(destroyAction);
			}
		}

		private void StartSwipe(CIcon aSelected, CIcon aTargeted) {
			var swipeAction = mController.mActionManager.createAction(EAction.eSwipe) as Actions.CSwipe;

			swipeAction.mSelectedIcon = aSelected;
			swipeAction.mTargetedIcon = aTargeted;

			if (mController.mActionManager.AddAction(swipeAction)) {
				mInput.Block();
			}
		}

		private bool IsActiveIcon (CIcon icon) {
			return icon != null && icon.IsMoveReady() && icon.IsInside();
		}
	}

}