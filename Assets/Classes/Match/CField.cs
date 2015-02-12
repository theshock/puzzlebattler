using Libraries;
using Match.Gem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Match {

	public class CField : MonoBehaviour {
		private CIcon [,] mIconMatrix;

		public int mRows {
			get { return mConfig.mField.mRows * 2; }
		}
		public int mColumns {
			get { return mConfig.mField.mColumns; }
		}
		public Vector2 mStartPoint {
			get { return mConfig.mField.mStartPoint; }
		}

		public Vector2 mOffset {
			get { return mConfig.mField.mOffset; }
		}

		public Config.Match.CMatch mConfig;
		public CMatch mMatch;

		public void InitMatchField() {
			mIconMatrix  = new CIcon[mRows, mColumns];

			for (int r = 0; r < mRows; r++) {
				for (int c = 0; c < mColumns; c++) {
					mIconMatrix[r, c] = null;
					CreateIconByPos(new CCell(r, c), GenIconType(), true);
				}
			}
		}

		public CMove SwipeIcons (CIcon aFirstIcon, CIcon aSecondIcon) {
			var move = new CMove(mConfig);
			SwipeIconsCells(aFirstIcon, aSecondIcon);
			move.addMove(aFirstIcon, GetIconCenterByCoord(aFirstIcon.mCell));
			move.addMove(aSecondIcon, GetIconCenterByCoord(aSecondIcon.mCell));
			return move;
		}

		public void SwipeIconsCells (CIcon aFirstIcon, CIcon aSecondIcon) {
			CCell cell = aFirstIcon.mCell.Clone();

			SetMatrixCell(aFirstIcon.mCell, aSecondIcon);
			SetMatrixCell(aSecondIcon.mCell, aFirstIcon);

			aFirstIcon .mCell.Set(aSecondIcon.mCell);
			aSecondIcon.mCell.Set(cell);
		}

		public CMove UpdatePositionIcons() {
			GravityIconsFall();
			FillFreeIcons();
			return EnsureIconsPosition();
		}

		public void SetMatrixCell (CCell position, CIcon icon) {
			mIconMatrix[position.row, position.col] = icon;
		}

		public CIcon GetMatrixCell (CCell position) {
			return mIconMatrix[position.row, position.col];
		}

		public List<CIcon> GetIconsByRow(int aRow) {
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

		public CIcon GetIconByIndex(int aIndex) {
			if (aIndex >= mRows * mColumns) {
				return null;
			}

			int row = aIndex / mColumns;
			int column = aIndex - row * mColumns;
			return mIconMatrix[row, column];
		}

		public bool AreIconsTheSame(List<CIcon> icons) {
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
			foreach (CIcon icon in mIconMatrix) {
				if (icon.IconState == EState.Clear) {
					return true;
				}
			}

			return false;
		}

		public Vector3 GetIconCenterByCoord(CCell cell) {
			return new Vector3(
				mStartPoint.x + cell.col * mOffset.x,
				mStartPoint.y + cell.row * mOffset.y,
				this.transform.position.z
			);
		}

		public CIcon GetIconByPosition(Vector2 aPos) {
			foreach (CIcon icon in mIconMatrix) {
				if (icon.HitTest(aPos)) {
					return icon;
				}
			}

			return null;
		}

		// private starts

		private EType GenIconType() {
			int count = Enum.GetNames( typeof( EType ) ).Length;

			return (EType) UnityEngine.Random.Range(0, count);
		}

		private void FillFreeIcons() {
			for ( int c = 0; c < mColumns; c++ ) {
				for ( int r = 0; r < mRows; r++ ) {
					if ( mIconMatrix[r, c].IconState == EState.Clear) {
						CreateIconByPos(new CCell(r, c), GenIconType(), false);
					}
				}
			}
		}

		private void GravityIconsFall() {
			// Find destroyed cells and search
			// for live cells in top of them
			for ( int col = 0; col < mColumns; col++ ) {
				for ( int row = 0; row < mRows - 1; row++ ) {
					CIcon current = mIconMatrix[row, col];

					if ( current.IconState != EState.Clear ) continue;

					for (int topRow = row + 1; topRow < mRows; topRow++) {
						CIcon top = mIconMatrix[topRow, col];

						if (top.IconState == EState.Clear) continue;

						SwipeIconsCells(current, top);
						break;
					}
				}
			}
		}

		private CMove EnsureIconsPosition () {
			var move = new CMove(mConfig);

			for ( int c = 0; c < mColumns; c++ ) {
				for ( int r = 0; r < mRows; r++ ) {
					move.addMove(mIconMatrix[r, c], GetIconCenterByCoord(new CCell(r, c)));
				}
			}

			return move;
		}

		private void CreateIconByPos(CCell aPosition, EType aIconType, bool aIsSetStartPosition) {
			CIcon icon = GetMatrixCell(aPosition);

			if (icon == null) {
				icon = CreateIcon();
				SetMatrixCell(aPosition, icon);
			}

			icon.mField = this;
			icon.mCell.Set(aPosition);
			icon.IconState = aPosition.row < mRows / 2 ? EState.Open : EState.Invisible;
			icon.gameObject.transform.position = GetIconCenterByCoord(
				aIsSetStartPosition ? aPosition : new CCell(aPosition.row + mRows / 2, aPosition.col)
			);

			if (aIsSetStartPosition) {
				do {
					icon.IconType = GenIconType();
				} while (CanCreateMatch(icon));
			} else {
				icon.IconType = aIconType;
			}
		}

		private bool CanCreateMatch (CIcon icon) {
			CCell cell = icon.mCell;

			if (cell.col >= 2 && AreIconsTheSame(
				new List<CIcon>{
					icon,
					GetMatrixCell(new CCell(cell.row, cell.col-1)),
					GetMatrixCell(new CCell(cell.row, cell.col-2))
				}
			)) {
				return true;
			}

			if (cell.row >= 2 && AreIconsTheSame(
				new List<CIcon>{
					icon,
					GetMatrixCell(new CCell(cell.row-1, cell.col)),
					GetMatrixCell(new CCell(cell.row-2, cell.col))
				}
			)) {
				return true;
			}

			return false;
		}

		private CIcon CreateIcon() {
			GameObject icon = Instantiate(mConfig.mGems.mPrefab) as GameObject;
			icon.transform.SetParent(this.transform);
			icon.transform.localScale = new Vector3(1,1,1);

			return icon.GetComponent<CIcon>();
		}
	}
}