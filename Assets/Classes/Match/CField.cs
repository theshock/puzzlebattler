using Libraries;
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
					CreateIconByPos(new CCell(r, c), EIconType.Count, true);
				}
			}

		}

		public void swipeCellInMatrix(CIcon aFirstIcon, CIcon aSecondIcon) {

			CCell cell = aFirstIcon.mCell.Clone();
			mIconMatrix[aFirstIcon.mCell.row, aFirstIcon.mCell.col] = aSecondIcon;
			mIconMatrix[aSecondIcon.mCell.row, aSecondIcon.mCell.col] = aFirstIcon;

			aFirstIcon .mCell.Set(aSecondIcon.mCell);
			aSecondIcon.mCell.Set(cell);
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
					if ( mIconMatrix[r, c].MoveTo(new CCell(r, c), delay) ) {
						mCountStartMoveCells++;
						delay += 0.01f;
					}
				}
			}

			return mCountStartMoveCells;
		}

		void CreateIconByPos(CCell aPosition, EIconType aIconType, bool aIsSetStartPosition) {
			if (aIconType == EIconType.Count) {
				aIconType = GenIconType();
			}

			CIcon icon = GetMatrixCell(aPosition);

			if (icon == null) {
				icon = createIcon();
				SetMatrixCell(aPosition, icon);
			}

			icon.mField = this;
			icon.mCell.Set(aPosition);
			icon.IconState = aPosition.row < mRows / 2 ? EIconState.eOpen : EIconState.eInvisible;
			icon.gameObject.transform.position = GetIconCenterByIndex(
				aIsSetStartPosition ? aPosition : new CCell(aPosition.row + mRows / 2, aPosition.col)
			);

			icon.IconType = aIconType;
		}

		public void SetMatrixCell (CCell position, CIcon icon) {
			mIconMatrix[position.row, position.col] = icon;
		}

		public CIcon GetMatrixCell (CCell position) {
			return mIconMatrix[position.row, position.col];
		}

		CIcon createIcon() {
			GameObject icon = Instantiate(mConfig.mGems.mPrefab) as GameObject;
			Transform transform = icon.transform;
			transform.SetParent(this.transform);
			transform.localScale = new Vector3(1,1,1);

			return icon.GetComponent<CIcon>();
		}

		void fillFreeIcons() {
			for ( int c = 0; c < mColumns; c++ ) {
				for ( int r = 0; r < mRows; r++ ) {
					if ( mIconMatrix[r, c].IconState == EIconState.eClear) {
						CreateIconByPos(new CCell(r, c), EIconType.Count, false);
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

		public Vector3 GetIconCenterByIndex(CCell cell) {
			return new Vector3(
				mStartPoint.x + cell.col * mOffset.x,
				mStartPoint.y + cell.row * mOffset.y,
				this.transform.position.z
			);
		}

		public CIcon getIconByPos(int aRow, int aColumn) {
			return GetMatrixCell(new CCell(aRow, aColumn));
		}

		public CIcon GetIconByPosition(Vector2 aPos) {
			for (int r = 0; r < mRows; r++) {
				for (int c = 0; c < mColumns; c++) {
					CIcon icon = mIconMatrix[r, c];

					if (icon && icon.HitTest(aPos) && icon.IsMoveReady()) {
						return icon;
					}
				}
			}

			return null;
		}

		public CIcon GetIconByIndex(int aIndex) {
			if (aIndex >= mRows * mColumns)
				return null;

			int row = aIndex / mColumns;
			int column = aIndex - row * mColumns;
			return mIconMatrix[row, column];
		}
	}
}