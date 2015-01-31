using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Match {

	public class CIcon : MonoBehaviour {
		float mTimeDelay = 0.1f;

		public delegate void SwipeDelegate();
		public SwipeDelegate mDelegate = null;

		private EIconType mIconType;
		public EIconType IconType {
			get {
				return mIconType;
			}
			set {
				mIconType = value;

				if (mIconSpriteRenderer) {
					Sprite spr = mMatchField.iconSprites[(int)mIconType];

					mIconSpriteRenderer.sprite = spr;
					mIconSpriteRenderer.SetNativeSize();
				}

			}
		}


		private EIconState mIconState;
		public EIconState IconState {
			get {
				return mIconState;
			}
			set {
				mIconState = value;

				if (mIconSpriteRenderer) {
					mIconSpriteRenderer.enabled = (mIconState != EIconState.eClear);
				} else {
					Debug.LogError("mIconSpriteRenderer fail");
				}

			}
		}

		public int mRow;
		public int mColumn;
		public int mIndex { get; set; }
		public Image mIconSpriteRenderer = null;
		public CField mMatchField { get; set; }

		public string getLogInfo() {
			return "Icon Info [Row : " + mRow + "] [mCol : " + mColumn + "] [State : " + mIconState + "]";
		}

		void Start() {
			if (mIconSpriteRenderer == null) {
				mIconSpriteRenderer = GetComponent<Image>();
			}
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
			Vector2 convert_pos = Camera.main.ScreenToWorldPoint(aPos);

			return collider2D.OverlapPoint(convert_pos);
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