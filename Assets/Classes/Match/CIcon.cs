﻿using Libraries;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Match {

	public class CIcon : MonoBehaviour {

		public delegate void SwipeDelegate();
		public SwipeDelegate mDelegate = null;
		public CIconMove mMover;

		private EIconType mIconType;
		public EIconType IconType {
			get {
				return mIconType;
			}
			set {
				mIconType = value;
				GetComponent<Image>().sprite = mField.mConfig.mGems.GetCorrectSprite(mIconType);
			}
		}

		private EIconState mIconState;
		public EIconState IconState {
			get {
				return mIconState;
			}
			set {
				mIconState = value;
				GetComponent<Image>().enabled = (mIconState != EIconState.eClear);
			}
		}


		public CField mField;
		public CCell mCell = CCell.zero;

		public int mIndex {
			get { return mCell.row * mField.mColumns + mCell.col; }
		}

		public void OnDestroyIcon() {
			GameObject anim = Instantiate(mField.mConfig.mDie.GetPrefab(mIconType), transform.position, transform.rotation) as GameObject;

			anim.transform.SetParent(transform.parent);

			Destroy(anim, 2);
		}

		public bool IsActionReady() {
			return (mIconState == EIconState.eOpen);
		}

		public bool IsInside () {
			return mCell.row < mField.mRows / 2;
		}

		public bool IsMoveReady() {
			return (mIconState == EIconState.eOpen);
		}

		public bool HitTest(Vector2 aCoordinates) {
			var worldPoint = Camera.main.ScreenToWorldPoint((Vector3) aCoordinates);

			return collider2D.OverlapPoint((Vector2) worldPoint);
		}

		public bool IsNeighbour (CIcon target) {
			if (this == target) return false;

			int rowDistance = Mathf.Abs(mCell.row - target.mCell.row);
			int colDistance = Mathf.Abs(mCell.col - target.mCell.col);

			return (rowDistance == 1 && colDistance == 0) || (rowDistance == 0 && colDistance == 1);
		}

		public void OnEndMoveComplete() {
			mMover.OnEndMoveComplete(this);
		}
	}
}