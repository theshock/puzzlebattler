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
			get { return mConfig.mField.mRows; }
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
			mIconMatrix  = new CIcon[mRows * 2, mColumns];

			for (int r = 0; r < mRows * 2; r++) {
				for (int c = 0; c < mColumns; c++) {
					mIconMatrix[r, c] = null;
					var pos  = new CCell(r, c);
					var icon = CreateIconByPos(pos);
					icon.gameObject.transform.position = GetIconCenterByCoord(pos);
					do {
						icon.SetRandomType();
					} while (CanCreateMatch(icon));
				}
			}
		}

		public CMove SwipeIcons (CIcon aFirstIcon, CIcon aSecondIcon) {
			var move = new CMove(mConfig);
			SwipeIconsCells(aFirstIcon, aSecondIcon);
			move.AddMove(aFirstIcon, GetIconCenterByCoord(aFirstIcon.mCell));
			move.AddMove(aSecondIcon, GetIconCenterByCoord(aSecondIcon.mCell));
			return move;
		}

		public void SwipeIconsCells (CIcon aFirstIcon, CIcon aSecondIcon) {
			CCell cell = aFirstIcon.mCell.Clone();

			SetMatrixCell(aFirstIcon.mCell, aSecondIcon);
			SetMatrixCell(aSecondIcon.mCell, aFirstIcon);

			aFirstIcon .mCell.Set(aSecondIcon.mCell);
			aSecondIcon.mCell.Set(cell);
		}

		public void SetMatrixCell (CCell position, CIcon icon) {
			mIconMatrix[position.row, position.col] = icon;
		}

		public CIcon GetMatrixCell (CCell position) {
			return mIconMatrix[position.row, position.col];
		}

		public CIcon GetIconByIndex(int aIndex) {
			if (aIndex >= mRows * 2 * mColumns) {
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
				if (icon.Type != icons[0].Type) {
					return false;
				}
			}

			return true;
		}

		public bool HasEmptyIcon() {
			foreach (CIcon icon in mIconMatrix) {
				if (icon.State == EState.Death) {
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

		private CIcon CreateIconByPos(CCell position) {
			CIcon icon = GetMatrixCell(position);

			if (icon == null) {
				icon = CreateIcon();
				SetMatrixCell(position, icon);
				icon.mField = this;
			}

			icon.mCell.Set(position);
			icon.State = EState.Idle;

			return icon;
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
			icon.transform.localScale = Vector3.one;

			return icon.GetComponent<CIcon>();
		}
	}
}