using System.Collections;
using UnityEngine;

namespace Match {

	public class CView : MonoBehaviour {
		public CField mField = null;
		public CMatch mController = null;

		private CIcon mSelectedIcon = null;
		private bool mIsIgnoreTouch = false;
		private bool mIsStartClick = false;

		void Start() {
			Input.simulateMouseWithTouches = true;
		}

		public void init() {
			mField.initMatchField();
		}

		void Update() {
			if (Input.touchCount == 1) {
				Touch touch = Input.GetTouch(0);

				switch (touch.phase) {
					case TouchPhase.Began:
						onTouchBegan(touch);
						break;

					case TouchPhase.Moved:
						onTouchMoved(touch);
						break;

					case TouchPhase.Ended:
						onTouchEnded(touch);
						break;
				}
			} else if ( Input.GetMouseButton(0) ) {
				if (!mIsStartClick) {
					mIsStartClick = true;
					onTouchBegan(Input.mousePosition);
				} else {
					onTouchMoved(Input.mousePosition);
				}
			} else if (mIsStartClick) {
				mIsStartClick = false;
				mSelectedIcon = null;
				mIsIgnoreTouch = false;
			}
		}

		public void destroyRow(int aRow) {
			ArrayList row = mField.getIconsByRow(aRow);

			foreach (CIcon icon in row) {
				Actions.IAction destroy_action = mController.mActionManager.createAction(EAction.eDestroy);
				Hashtable hash = new Hashtable();
				hash.Add("target", icon);
				destroy_action.doUpdateActionParam(hash);

				mController.mActionManager.addAction(destroy_action);
			}
		}

		public void destroyColumn(int aCol) {
			ArrayList column = mField.getIconsByColumn(aCol);

			foreach (CIcon icon in column) {
				Actions.IAction destroy_action = mController.mActionManager.createAction(EAction.eDestroy);
				Hashtable hash = new Hashtable();
				hash.Add("target", icon);
				destroy_action.doUpdateActionParam(hash);

				mController.mActionManager.addAction(destroy_action);
			}
		}

		public void destroySector(Rect aSize) {

		}

		public void destroyIconsByType(EIconType aType) {

		}

		void onTouchBegan(Vector2 aPos) {
			if (mIsIgnoreTouch)
				return;

			Vector2 toush_pos = aPos;
			mSelectedIcon = mField.getIconByPos(toush_pos);
		}

		void onTouchMoved(Vector2 aPos) {
			Vector2 point = aPos;

			if (mSelectedIcon && mSelectedIcon.getIsReadyMove() && !mIsIgnoreTouch) {
				CIcon target_icon = mField.getIconByPos(point);

				if (target_icon && target_icon.getIsReadyMove() && mSelectedIcon != target_icon) {
					int rowDistance = Mathf.Abs(mSelectedIcon.mRow - target_icon.mRow);
					int colDistance = Mathf.Abs(mSelectedIcon.mColumn - target_icon.mColumn);

					if ((rowDistance == 1 && colDistance == 0) || (rowDistance == 0 && colDistance == 1)) {
						Actions.IAction swipe_action = mController.mActionManager.createAction(EAction.eSwipe);

						Hashtable hash = new Hashtable();
						hash.Add("selectedIcon", mSelectedIcon);
						hash.Add("targetedIcon", target_icon);
						swipe_action.doUpdateActionParam(hash);

						int res = mController.mActionManager.addAction(swipe_action);

						if (res == 1) {
							mIsIgnoreTouch = true;
						}
					}

					mSelectedIcon = null;
				}
			}

		}

		void onTouchBegan(Touch aTouch) {
			if (mIsIgnoreTouch)
				return;

			Vector2 toush_pos = aTouch.position;
			mSelectedIcon = mField.getIconByPos(toush_pos);
		}

		void onTouchMoved(Touch aTouch) {
			Vector2 point = aTouch.position;

			if (mSelectedIcon && mSelectedIcon.getIsReadyMove() && !mIsIgnoreTouch) {
				CIcon target_icon = mField.getIconByPos(point);

				if (target_icon && target_icon.getIsReadyMove() && mSelectedIcon != target_icon) {
					int rowDistance = Mathf.Abs(mSelectedIcon.mRow - target_icon.mRow);
					int colDistance = Mathf.Abs(mSelectedIcon.mColumn - target_icon.mColumn);

					if ((rowDistance == 1 && colDistance == 0) || (rowDistance == 0 && colDistance == 1)) {
						Actions.IAction swipe_action = mController.mActionManager.createAction(EAction.eSwipe);
						Hashtable hash = new Hashtable();
						hash.Add("selectedIcon", mSelectedIcon);
						hash.Add("targetedIcon", target_icon);
						swipe_action.doUpdateActionParam(hash);

						mController.mActionManager.addAction(swipe_action);

						mIsIgnoreTouch = true;
					}

					mSelectedIcon = null;
				}
			}

		}

		void onTouchEnded(Touch aTouch) {
			mSelectedIcon = null;
			mIsIgnoreTouch = false;
		}
	}

}