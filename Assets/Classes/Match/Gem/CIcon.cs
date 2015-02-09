using Libraries;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Match.Gem {

	public class CIcon : MonoBehaviour {

		public CMove mMover;

		private EType mIconType;
		public EType IconType {
			get {
				return mIconType;
			}
			set {
				mIconType = value;
				GetComponent<Image>().sprite = mField.mConfig.mGems.GetCorrectSprite(mIconType);
			}
		}

		private EState mIconState;
		public EState IconState {
			get {
				return mIconState;
			}
			set {
				mIconState = value;
				GetComponent<Image>().enabled = (mIconState != EState.Clear);
			}
		}

		public CField mField;
		public CCell mCell = CCell.zero;

		public int mIndex {
			get { return mCell.row * mField.mColumns + mCell.col; }
		}

		public void TmpHL () {
			var prefab = mField.mConfig.mDie.GetPrefab(IconType);
			GameObject anim = Instantiate(prefab, transform.position, transform.rotation) as GameObject;

			anim.transform.SetParent(transform.parent);

			Destroy(anim, 10);
		}

		public bool IsActionReady() {
			return (mIconState == EState.Open);
		}

		public bool IsInside () {
			return mCell.row < mField.mRows / 2;
		}

		public bool IsMoveReady() {
			return (mIconState == EState.Open);
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