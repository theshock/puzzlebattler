using System.Collections;
using UnityEngine;

namespace Match {

	public class CView : MonoBehaviour, IInputObserver {
		public CField mField = null;
		public CMatch mController = null;

		private CInput mInput;
		private CIcon mSelectedIcon = null;

		void Start() {
			mInput = new CInput(this);
			Input.simulateMouseWithTouches = true;
		}

		public void init() {
			mField.initMatchField();
		}

		public void Update() {
			mInput.Check();
		}

		public void destroyRow(int aRow) {
			DestroyList(mField.getIconsByRow(aRow));
		}

		public void destroyColumn(int aCol) {
			DestroyList(mField.getIconsByColumn(aCol));
		}

		public void OnInputBegin (Vector2 aPosition) {
			mSelectedIcon = mField.GetIconByPosition(aPosition);
		}

		public void OnInputMove (Vector2 aPosition) {
			if (!IsActiveIcon(mSelectedIcon)) return;

			CIcon targetIcon = mField.GetIconByPosition(aPosition);

			if (IsActiveIcon(targetIcon) && mSelectedIcon != targetIcon) {
				if (mSelectedIcon.IsNeighbour(targetIcon)) {
					startSwipe(mSelectedIcon, targetIcon);
				}

				mSelectedIcon = null;
			}

		}

		public void OnInputEnd () {
			mSelectedIcon = null;
		}

		private void DestroyList (ArrayList aIcons) {
			foreach (CIcon icon in aIcons) {
				var destroyAction = mController.mActionManager.createAction(EAction.eDestroy) as Actions.CDestroy;
				destroyAction.configure(icon);
				mController.mActionManager.addAction(destroyAction);
			}
		}

		private void startSwipe (CIcon aSelected, CIcon aTargeted) {
			var swipeAction = mController.mActionManager.createAction(EAction.eSwipe) as Actions.CSwipe;

			swipeAction.configure(aSelected, aTargeted);

			if (mController.mActionManager.addAction(swipeAction) == 1) {
				mInput.Block();
			}
		}

		private bool IsActiveIcon (CIcon aIcon) {
			return aIcon != null && aIcon.getIsReadyMove();
		}
	}

}