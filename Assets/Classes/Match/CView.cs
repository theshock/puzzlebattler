
using System.Collections;
using UnityEngine;

namespace Match {

	public class CView : MonoBehaviour {
		public CField mMatchField = null;
		public CController mMatchController = null;

		private CIcon mSelectedIcon = null;
		private bool mIsIgnoreTouch = false;
		private bool mIsStartClick = false;

		// Use this for initialization
		void Start() {
			Input.simulateMouseWithTouches = true;
		}

		// Update is called once per frame

		public void init() {
			mMatchField.initMatchField();
		}

		void Update() {
			if (Input.touchCount == 1) {
				Touch touch = Input.GetTouch(0);

				switch (touch.phase) {
					case TouchPhase.Began:
				{
					onTouchBegan(touch);
				}
					break;

					case TouchPhase.Moved:
				{
					onTouchMoved(touch);
				}
					break;

					case TouchPhase.Ended:
				{
					onTouchEnded(touch);
				}
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
				//			Debug.Log( "Click End" );

				mIsStartClick = false;
				mSelectedIcon = null;
				mIsIgnoreTouch = false;
			}
		}

		public void destroyRow(int aRow) {
			ArrayList row = mMatchField.getIconsByRow(aRow);

			foreach (CIcon icon in row) {
				Actions.IAction destroy_action = mMatchController.mActionManager.createAction(EMatchAction.eDestroyAction);
				Hashtable hash = new Hashtable();
				hash.Add("target", icon);
				destroy_action.doUpdateActionParam(hash);

				mMatchController.mActionManager.addAction(destroy_action);
			}
		}

		public void destroyColumn(int aCol) {
			ArrayList column = mMatchField.getIconsByColumn(aCol);

			foreach (CIcon icon in column) {
				Actions.IAction destroy_action = mMatchController.mActionManager.createAction(EMatchAction.eDestroyAction);
				Hashtable hash = new Hashtable();
				hash.Add("target", icon);
				destroy_action.doUpdateActionParam(hash);

				mMatchController.mActionManager.addAction(destroy_action);
			}
		}

		public void destroySector(Rect aSize) {

		}

		public void destroyIconsByType(EIconType aType) {

		}

		void onTouchBegan(Vector2 aPos) {
			if (mIsIgnoreTouch)
				return;

			//		Debug.Log("onTouchBegan");
			Vector2 toush_pos = aPos;
			mSelectedIcon = mMatchField.getIconByPos(toush_pos);
		}

		void onTouchMoved(Vector2 aPos) {
			//		Debug.Log("onTouchMoved");
			Vector2 point = aPos;

			if (mSelectedIcon && mSelectedIcon.getIsReadyMove() && !mIsIgnoreTouch) {
				CIcon target_icon = mMatchField.getIconByPos(point);

				if (target_icon && target_icon.getIsReadyMove() && mSelectedIcon != target_icon) {
					int rowDistance = Mathf.Abs(mSelectedIcon.mRow - target_icon.mRow);
					int colDistance = Mathf.Abs(mSelectedIcon.mColumn - target_icon.mColumn);

					//				Debug.Log("[rowDistance : " + rowDistance + "]");
					//				Debug.Log("[colDistance : " + colDistance + "]");

					if ((rowDistance == 1 && colDistance == 0) || (rowDistance == 0 && colDistance == 1)) {
						Actions.IAction swipe_action = mMatchController.mActionManager.createAction(EMatchAction.eSwipeAction);

						Hashtable hash = new Hashtable();
						hash.Add("selectedIcon", mSelectedIcon);
						hash.Add("targetedIcon", target_icon);
						swipe_action.doUpdateActionParam(hash);

						int res = mMatchController.mActionManager.addAction(swipe_action);

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
			mSelectedIcon = mMatchField.getIconByPos(toush_pos);
		}

		void onTouchMoved(Touch aTouch) {
			Vector2 point = aTouch.position;

			if (mSelectedIcon && mSelectedIcon.getIsReadyMove() && !mIsIgnoreTouch) {
				CIcon target_icon = mMatchField.getIconByPos(point);

				if (target_icon && target_icon.getIsReadyMove() && mSelectedIcon != target_icon) {
					int rowDistance = Mathf.Abs(mSelectedIcon.mRow - target_icon.mRow);
					int colDistance = Mathf.Abs(mSelectedIcon.mColumn - target_icon.mColumn);

					if ((rowDistance == 1 && colDistance == 0) || (rowDistance == 0 && colDistance == 1)) {
						Actions.IAction swipe_action = mMatchController.mActionManager.createAction(EMatchAction.eSwipeAction);
						Hashtable hash = new Hashtable();
						hash.Add("selectedIcon", mSelectedIcon);
						hash.Add("targetedIcon", target_icon);
						swipe_action.doUpdateActionParam(hash);

						mMatchController.mActionManager.addAction(swipe_action);

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