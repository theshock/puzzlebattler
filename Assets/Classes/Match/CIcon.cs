using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Match {

	public class CIcon : MonoBehaviour {
		float mTimeDelay = 0.1f;

		public delegate void SwipeDelegate();
		public SwipeDelegate mDelegate = null;

		public Sprite mImageRed;
		public Sprite mImageBlue;
		public Sprite mImageYellow;
		public Sprite mImageGreen;
		public Sprite mImagePurple;

		private EIconType mIconType;
		public EIconType IconType {
			get {
				return mIconType;
			}
			set {
				mIconType = value;
				GetComponent<Image>().sprite = GetCorrectSprite();
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

		public int mRow;
		public int mColumn;
		public int mIndex { get; set; }
		public CField mMatchField { get; set; }
		private Image mIconSpriteRenderer = null;

		private Sprite GetCorrectSprite () {
			switch (mIconType) {
				case EIconType.eRed   : return mImageRed;
				case EIconType.eBlue  : return mImageBlue;
				case EIconType.eGreen : return mImageGreen;
				case EIconType.ePurple: return mImagePurple;
				case EIconType.eYellow: return mImageYellow;
			}

			return null;
		}

		public void onDestroyIcon() {
			GameObject anim = Instantiate(mMatchField.mPrefab[(int)mIconType], transform.position, transform.rotation) as GameObject;

			anim.transform.SetParent(transform.parent);

			Destroy(anim, 2);
		}

		public bool getIsReadyAction() {
			return (mIconState == EIconState.eOpen);
		}

		public bool getIsReadyMove() {
			return (mIconState == EIconState.eOpen);
		}

		public bool hitTest(Vector2 aPos) {
			return collider2D.OverlapPoint(Camera.main.ScreenToWorldPoint(aPos));
		}

		public void initWithParams(CField aMatchField, Vector2 aIconPos, EIconType aIconType, int aIndex) {
			mMatchField = aMatchField;
			mRow = (int)aIconPos.x;
			mColumn = (int) aIconPos.y;
			mIndex = aIndex;
		}

		public void onEndSwipeAnimation() {
			if (mDelegate != null) {
				mDelegate.Invoke();
			}
		}

		public bool IsNeighbour (CIcon target) {
			int rowDistance = Mathf.Abs(mRow - target.mRow);
			int colDistance = Mathf.Abs(mColumn - target.mColumn);

			return (rowDistance == 1 && colDistance == 0) || (rowDistance == 0 && colDistance == 1);
		}

		public bool moveTo(int aRow, int aCol, float aDelay) {
			Vector2 pos = mMatchField.getIconCenterByIndex(aRow, aCol);
			Vector3 pos3d = new Vector3(pos.x, pos.y, this.transform.position.z);

			if (this.transform.position == pos3d)
				return false;

			mIconState = EIconState.eLock;
			iTween.MoveTo(this.gameObject, iTween.Hash("position", pos3d, "time", mTimeDelay, "onComplete", "onEndMoveComplete"));

			return true;
		}

		public void onEndMoveComplete() {
			mIconState = EIconState.eOpen;
			mMatchField.onEndMoveComplete(this);
		}
	}
}