using Libraries;
using Libraries.ActionSystem;
using Match.Gem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match.Actions {
	public class CRefreshPosition : CBase, IMoveObserver {
		protected CField field;

		public CRefreshPosition (CField field) {
			this.field = field;
		}

		public override bool Validation() {
			return field.HasIconsWithState(EState.Death);
		}

		private void DeadFreeFall (int c, List<CIcon> dead, CMove move) {
			int number = 0;
			int height = field.mRows;

			foreach (CIcon icon in dead) {
				field.SetMatrixCell(new CCell(height - dead.Count + number, c), icon);

				icon.SetState(EState.Idle);
				icon.gameObject.transform.position = field.GetIconCenterByCoord(
					new CCell(height + number, c)
				);
				icon.SetRandomColor();
				move.AddMove( icon, field.GetIconCenterByCoord(icon.mCell) );

				number++;
			}
		}

		private void ColumnFreeFall (int c, CMove move) {
			List<CIcon> dead = new List<CIcon>();

			for ( int r = 0; r < field.mRows; r++ ) {
				CIcon current = field.GetMatrixCell(new CCell(r, c));

				if (current.state == EState.Death) {
					dead.Add(current);
					continue;
				} else if (dead.Count == 0) {
					continue; // do nothing - current now at correct place
				}

				field.SetMatrixCell(new CCell(r - dead.Count, c), current);

				move.AddMove( current, field.GetIconCenterByCoord(current.mCell) );
			}

			DeadFreeFall(c, dead, move);
		}

		public override void StartAction() {
			var move = new CMove(field.mConfig);

			for ( int col = 0; col < field.mColumns; col++ ) {
				ColumnFreeFall(col, move);
			}

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