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
			mIconMatrix = new CIcon[mRows, mColumns];

			for (int r = 0; r < mRows; r++) {
				for (int c = 0; c < mColumns; c++) {
					CreateIconByPos(new CCell(r, c));
				}
			}
		}

		public void SetMatrixCell (CCell position, CIcon icon) {
			mIconMatrix[position.row, position.col] = icon;
			icon.mCell.Set(position);
		}

		public CIcon GetMatrixCell (CCell position) {
			return mIconMatrix[position.row, position.col];
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
				if (icon.Type != icons[0].Type) {
					return false;
				}
			}

			return true;
		}

		public bool HasIconsWithState (EState state) {
			foreach (CIcon icon in mIconMatrix) {
				if (icon.State == state) {
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
			CIcon icon = CreateIcon();
			SetMatrixCell(position, icon);

			icon.mField = this;
			icon.State = EState.Idle;
			icon.gameObject.transform.position = GetIconCenterByCoord(position);

			do {
				icon.SetRandomType();
			} while (CanCreateMatch(icon));

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