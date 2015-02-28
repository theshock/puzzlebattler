using Libraries;
using Libraries.ActionSystem;
using Match.Gem;
using System.Collections;
using UnityEngine;

namespace Match.Actions {
	public class CRefreshPosition : CBase, IMoveObserver {
		protected CField field;

		public CRefreshPosition (CField field) {
			this.field = field;
		}

		public override bool Validation() {
			return field.HasEmptyIcon();
		}

		private void FillFreeIcons() {
			for ( int c = 0; c < field.mColumns; c++ ) {
				for ( int r = 0; r < field.mRows * 2; r++ ) {
					if ( field.GetMatrixCell(new CCell(r, c)).State == EState.Death) {
						var pos  = new CCell(r, c);
						var icon = field.GetMatrixCell(pos);
						icon.mCell.Set(pos);
						icon.State = EState.Idle;
						icon.gameObject.transform.position = field.GetIconCenterByCoord(
							new CCell(pos.row + field.mRows, pos.col)
						);
						icon.SetRandomType();
					}
				}
			}
		}

		private void GravityIconsFall() {
			// Find destroyed cells and search
			// for live cells in top of them
			for ( int col = 0; col < field.mColumns; col++ ) {
				for ( int row = 0; row < field.mRows * 2 - 1; row++ ) {
					CIcon current = field.GetMatrixCell(new CCell(row, col));

					if ( current.State != EState.Death ) continue;

					for (int topRow = row + 1; topRow < field.mRows * 2; topRow++) {
						CIcon top = field.GetMatrixCell(new CCell(topRow, col));

						if (top.State == EState.Death) continue;

						field.SwipeIconsCells(current, top);
						break;
					}
				}
			}
		}

		private CMove EnsureIconsPosition () {
			var move = new CMove(field.mConfig);

			for ( int c = 0; c < field.mColumns; c++ ) {
				for ( int r = 0; r < field.mRows * 2; r++ ) {
					move.AddMove(
						field.GetMatrixCell(new CCell(r, c)),
						field.GetIconCenterByCoord(new CCell(r, c))
					);
				}
			}

			return move;
		}

		public override void StartAction() {
			Debug.Log("Start refresh");

			GravityIconsFall();
			FillFreeIcons();

			var move = EnsureIconsPosition();

			if (move.IsFinished()) {
				ComplateAction();
			} else {
				move.SetObserver(this);
			}
		}

		public void OnMoveEnd () {
			ComplateAction();
		}

		public override int GetIndex() {
			return (int) EEvents.RefreshPosition;
		}
	}

}