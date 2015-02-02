using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Match {

	public class CField : MonoBehaviour {
		private CIcon [,] mIconMatrix;

		public int mRows {
			get {
				return mConfig.mField.mRows * 2;
			}
		}
		public int mColumns {
			get {
				return mConfig.mField.mColumns;
			}
		}
		public Vector2 mStartPoint {
			get {
				return mConfig.mField.mStartPoint;
			}
		}

		public Vector2 mOffset {
			get {
				return mConfig.mField.mOffset;
			}
		}

		public delegate void UpdatePositionDelegate();
		private UpdatePositionDelegate mDelegate;
		private int mCountStartMoveCells;
		public Config.Match.CMatch mConfig;

		public void initMatchField() {
			mIconMatrix = new CIcon[mRows, mColumns];

			for (int r = 0; r < mRows; r++) {
				for (int c = 0; c < mColumns; c++) {
					mIconMatrix[r, c] = null;
					CreateIconByPos(new Vector2(r, c), EIconType.Count, true);
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

		EIconType GenIconType() {
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

		void CreateIconByPos(Vector2 aIconPos, EIconType aIconType, bool aIsSetStartPosition) {
			if (aIconType == EIconType.Count) {
				aIconType = GenIconType();
			}

			int row = (int) aIconPos.x;
			int col = (int) aIconPos.y;

			if (mIconMatrix[row, col] == null) {
				mIconMatrix[row, col] = createIcon().GetComponent<CIcon>();
			}

			CIcon icon = mIconMatrix[row, col];

			icon.initWithParams(this, new Vector2(row, col), aIconType, row * mColumns + col);

			icon.IconState = row < mRows / 2 ? EIconState.eOpen : EIconState.eInvisible;
			icon.gameObject.transform.position = getIconCenterByIndex(aIsSetStartPosition ? row : row + mRows / 2, col);
			icon.gameObject.transform.localScale = new Vector3(1,1,1);

			icon.IconType = aIconType;
		}

		GameObject createIcon() {
			GameObject icon = Instantiate(mConfig.mGems.mPrefab) as GameObject;

			icon.transform.SetParent(this.transform);

			return icon;
		}

		void fillFreeIcons() {
			for ( int c = 0; c < mColumns; c++ ) {
				for ( int r = 0; r < mRows; r++ ) {
					if ( mIconMatrix[r, c].IconState == EIconState.eClear) {
						CreateIconByPos(new Vector2(r, c), EIconType.Count, false);
					}
				}
			}
		}

		public List<CIcon> getIconsByRow(int aRow) {
			var icons = new List<CIcon>();

			for (int c = 0; c < mColumns; c++) {
				icons.Add(mIconMatrix[aRow, c]);
			}

			return icons;
		}

		public List<CIcon> GetIconsByColumn(int aColumn) {
			var icons = new List<CIcon>();

			for (int r = 0; r < mRows; r++) {
				icons.Add(mIconMatrix[r, aColumn]);
			}

			return icons;
		}

		public bool IsIconsTheSame (List<CIcon> icons) {
			if (icons.Count < 1) {
				return false;
			}

			foreach (CIcon icon in icons) {
				if (icon.IconType != icons[0].IconType) {
					return false;
				}
			}

			return true;
		}

		public bool HasEmptyIcon() {
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

			return getIconCenterByIndex(row, column);
		}

		public CIcon getIconByPos(int aRow, int aColumn) {
			return mIconMatrix[aRow, aColumn];
		}

		public CIcon GetIconByPosition(Vector2 aPos) {
			for (int r = 0; r < mRows; r++) {
				for (int c = 0; c < mColumns; c++) {
					CIcon icon = mIconMatrix[r, c];

					if (icon && icon.hitTest(aPos) && icon.IsMoveReady()) {
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