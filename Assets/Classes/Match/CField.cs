using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Match {

	public class CField : MonoBehaviour {
		private CIcon [,] mIconMatrix;
		public int mRows;
		public int mColumns;
		public Vector2 mStartPoint;
		public Vector2 mOffset;
		public delegate void UpdatePositionDelegate();
		private UpdatePositionDelegate mDelegate;
		private int mCountStartMoveCells;
		public Sprite[] iconSprites;
		public GameObject[] mPrefab = null;

		void Awake() {
			iconSprites = new Sprite[(int)EIconType.Count];
			// load all frames in fruitsSprites array
			for (int i = 0; i < (int)EIconType.Count; i++) {
				string name = "match_icon/match_icon_" + i;
				var spr = Resources.Load<Sprite>(name);
				iconSprites[i] = spr;
			}
		}

		public void initMatchField() {
			mIconMatrix = new CIcon[mRows, mColumns];

			for (int r = 0; r < mRows; r++) {
				for (int c = 0; c < mColumns; c++) {
					mIconMatrix[r, c] = null;
					createIconByPos(new Vector2(r, c), EIconType.Count, true);
				}
			}

		}

		public void swipeCellInMatrix(CIcon aFirstIcon, CIcon aSecondIcon) {

			int row    = aFirstIcon.mRow;
			int column = aFirstIcon.mColumn;
			int index  = aFirstIcon.mIndex;

			mIconMatrix[aFirstIcon.mRow, aFirstIcon.mColumn] = aSecondIcon;
			mIconMatrix[aSecondIcon.mRow, aSecondIcon.mColumn] = aFirstIcon;

			aFirstIcon.mRow = aSecondIcon.mRow;
			aFirstIcon.mColumn = aSecondIcon.mColumn;
			aFirstIcon.mIndex = aSecondIcon.mIndex;

			aSecondIcon.mRow = row;
			aSecondIcon.mColumn = column;
			aSecondIcon.mIndex = index;

		}

		EIconType genIconType() {
			int count = EIconType.Count.GetHashCode();
			int type = Random.Range(0, count);

			EIconType res = (EIconType) type;

			return res;
		}

		public void onEndMoveComplete(CIcon aIcon) {
			mCountStartMoveCells--;

			if (mCountStartMoveCells == 0) {
				mDelegate.Invoke();
			}
		}

		public int updatePositionIcons(UpdatePositionDelegate aDelegate) {
			mCountStartMoveCells = 0;
			mDelegate = aDelegate;

			for ( int c = 0; c < mColumns; c++ ) {
				bool moved = true;
				while (moved) {
					moved = false;
					for ( int r = 0; r < mRows; r++ ) {
						if ( mIconMatrix[r, c].IconState == EIconState.eClear && (r + 1) < mRows && mIconMatrix[r + 1, c].IconState != EIconState.eClear ) {
							swipeCellInMatrix(mIconMatrix[r, c], mIconMatrix[r + 1, c]);

							moved = true;
						}
					}
				}
			}

			fillFreeIcons();

			for ( int c = 0; c < mColumns; c++ ) {
				float delay = 0;
				for ( int r = 0; r < mRows; r++ ) {
					if ( mIconMatrix[r, c].moveTo(r, c, delay) ) {
						mCountStartMoveCells++;
						delay += 0.01f;
					}
				}
			}

			return mCountStartMoveCells;
		}

		void createIconByPos(Vector2 aIconPos, EIconType aIconType, bool aIsSetStartPosition) {
			if (aIconType == EIconType.Count) {
				aIconType = genIconType();
			}

			int r = (int)aIconPos.x;
			int c = (int)aIconPos.y;

			if (mIconMatrix[r, c] == null) {
				//			Debug.Log("createIconByPos new Object");

				GameObject icon = createIcon();
				CIcon component = icon.GetComponent<CIcon>();
				Image render = icon.GetComponent<Image>();
				BoxCollider2D collider = icon.GetComponent<BoxCollider2D>();

				collider.size = new Vector2(157.0f, 158.0f);

				mIconMatrix[r, c] = component;

				component.initWithParams(this, new Vector2(r, c), aIconType, r * mColumns + c);
				component.mIconSpriteRenderer = render;

				if (r < mRows / 2) {
					component.IconState = EIconState.eOpen;
				} else {
					component.IconState = EIconState.eInvisible;
				}


				//			icon.transform.SetParent(this.transform);
				icon.transform.localScale = new Vector3(1, 1, 1);
				if (aIsSetStartPosition) {
					icon.transform.position = getIconCenterByIndex(r, c);
				} else {
					icon.transform.position = getIconCenterByIndex(r + mRows / 2, c);
				}

				component.IconType = aIconType;
			} else {
				//			Debug.Log("createIconByPos reinit Object");

				CIcon component = mIconMatrix[r, c];
				GameObject icon = component.gameObject;

				component.initWithParams(this, new Vector2(r, c), aIconType, r * mColumns + c);

				if (r < mRows / 2) {
					component.IconState = EIconState.eOpen;
				} else {
					component.IconState = EIconState.eInvisible;
				}

				if (aIsSetStartPosition) {
					icon.transform.position = getIconCenterByIndex(r, c);
				} else {
					icon.transform.position = getIconCenterByIndex(r + mRows / 2, c);
				}

				component.IconType = aIconType;
			}


		}

		GameObject createIcon() {
			GameObject icon = new GameObject();

			icon.AddComponent<CanvasRenderer>();
			icon.AddComponent<BoxCollider2D>();
			icon.AddComponent<Image>();
			icon.AddComponent("CIcon");
			icon.transform.SetParent(this.transform);

			return icon;
		}

		void fillFreeIcons() {
			for ( int c = 0; c < mColumns; c++ ) {
				for ( int r = 0; r < mRows; r++ ) {
					if ( mIconMatrix[r, c].IconState == EIconState.eClear) {
						createIconByPos(new Vector2(r, c), EIconType.Count, false);
					}
				}
			}
		}

		public ArrayList getIconsByRow(int aRow) {
			ArrayList res = new ArrayList();

			for (int c = 0; c < mColumns; c++) {
				res.Add(mIconMatrix[aRow, c]);
			}


			return res;
		}

		public ArrayList getIconsByColumn(int aColumn) {
			ArrayList res = new ArrayList();

			for (int r = 0; r < mRows; r++) {
				res.Add(mIconMatrix[r, aColumn]);
			}

			return res;
		}

		// todo: merge with isTheSameIconOne, iconsInTheSameCol, isTheSameIconOne
		public bool iconsInTheSameRow(ArrayList aSlice) {
			if (aSlice.Count < 1)
				return false;

			CIcon first_icon = aSlice[0] as CIcon;

			int standart = first_icon.mRow;
			int count = 0;

			foreach (CIcon icon in aSlice) {
				if (icon.mRow == standart) {
					count++;
				}
			}

			bool res = (count == aSlice.Count);

			return res;
		}

		public bool iconsInTheSameCol(ArrayList aSlice) {
			if (aSlice.Count < 1)
				return false;

			CIcon first_icon = aSlice[0] as CIcon;

			int standart = first_icon.mColumn;
			int count = 0;

			foreach (CIcon icon in aSlice) {
				if (icon.mColumn == standart) {
					count++;
				}
			}

			return (count == aSlice.Count);
		}

		public bool IsIconsTheSame (List<CIcon> icons) {
			if (icons.Count < 1) {
				return false;
			}

			int count = 0;

			foreach (CIcon icon in icons) {
				if (icon.IconType == icons[0].IconType) {
					count++;
				}
			}

			return (count == icons.Count);
		}

		public bool isTheSameIconOne(ArrayList aSlice) {
			if (aSlice.Count < 1)
				return false;

			CIcon first_icon = aSlice[0] as CIcon;

			EIconType standart = first_icon.IconType;
			int count = 0;

			foreach (CIcon icon in aSlice) {
				if (icon.IconType == standart) {
					count++;
				}
			}

			return (count == aSlice.Count);
		}

		public bool isHaveEmptyIcon() {
			for (int r = 0; r < mRows; r++) {
				for (int c = 0; c < mColumns; c++) {
					if (mIconMatrix[r, c].IconState == EIconState.eClear) {
						return true;
					}
				}
			}

			return false;
		}

		public Vector3 getIconCenterByIndex(int aRow, int aColumn) {
			return new Vector3(mStartPoint.x + aColumn * mOffset.x, mStartPoint.y + aRow * mOffset.y, 0);
		}

		public Vector3 getIconCenterByIndex(int aIndex) {
			int row = aIndex / mRows;
			int column = aIndex - row * mColumns;

			return new Vector3(mStartPoint.x + column * mOffset.x, mStartPoint.y + row * mOffset.y, 0);
		}

		public CIcon getIconByPos(int aRow, int aColumn) {
			return mIconMatrix[aRow, aColumn];
		}

		public CIcon GetIconByPosition(Vector2 aPos) {
			for (int r = 0; r < mRows; r++) {
				for (int c = 0; c < mColumns; c++) {
					CIcon icon = mIconMatrix[r, c];

					if (icon && icon.hitTest(aPos) && icon.getIsReadyMove()) {
						return icon;
					}
				}
			}

			return null;
		}

		public CIcon getIconByIndex(int aIndex) {
			if (aIndex >= mRows * mColumns)
				return null;

			int row = aIndex / mColumns;
			int column = aIndex - row * mColumns;
			return mIconMatrix[row, column];
		}
	}
}