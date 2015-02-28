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

		public Config.Match.CMatch mConfig;
		public CMatch mMatch;

		public void InitMatchField() {
			mIconMatrix = new CIcon[mRows, mColumns];

			for (int r = 0; r < mRows; r++) {
				for (int c = 0; c < mColumns; c++) {
					CreateIconByCoord(new CCell(r, c));
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
				if (icon.color != icons[0].color) {
					return false;
				}
			}

			return true;
		}

		public bool HasIconsWithState (EState state) {
			foreach (CIcon icon in mIconMatrix) {
				if (icon.state == state) {
					return true;
				}
			}

			return false;
		}

		public Vector3 GetIconCenterByCoord(CCell cell) {
			return new Vector3(
				mConfig.mField.mStartPoint.x + cell.col * mConfig.mField.mOffset.x,
				mConfig.mField.mStartPoint.y + cell.row * mConfig.mField.mOffset.y,
				this.transform.position.z
			);
		}

		public CIcon GetIconByPosition(Vector2 position) {
			foreach (CIcon icon in mIconMatrix) {
				if (icon.HitTest(position)) {
					return icon;
				}
			}

			return null;
		}

		private CIcon CreateIconByCoord(CCell position) {
			GameObject gameObject = Instantiate(mConfig.mGems.mPrefab) as GameObject;
			gameObject.transform.SetParent(this.transform);
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.position = GetIconCenterByCoord(position);

			CIcon icon = gameObject.GetComponent<CIcon>();

			SetMatrixCell(position, icon);

			icon.mField = this;
			icon.SetState(EState.Idle);

			do {
				icon.SetRandomColor();
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

	}
}