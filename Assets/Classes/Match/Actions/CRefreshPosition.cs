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
				icon.mCell.Set(new CCell(height - dead.Count + number, c));
				field.SetMatrixCell(icon.mCell, icon);
				icon.State = EState.Idle;
				icon.gameObject.transform.position = field.GetIconCenterByCoord(
					new CCell(height + number, c)
				);
				icon.SetRandomType();
				move.AddMove( icon, field.GetIconCenterByCoord(icon.mCell) );

				number++;
			}
		}

		private void ColumnFreeFall (int c, CMove move) {
			List<CIcon> dead = new List<CIcon>();

			for ( int r = 0; r < field.mRows; r++ ) {
				CIcon current = field.GetMatrixCell(new CCell(r, c));

				if (current.State == EState.Death) {
					dead.Add(current);
					continue;
				} else if (dead.Count == 0) {
					continue; // do nothing - current now at correct place
				}

				current.mCell.Set(new CCell(r - dead.Count, c));
				field.SetMatrixCell(current.mCell, current);

				move.AddMove( current, field.GetIconCenterByCoord(current.mCell) );
			}

			DeadFreeFall(c, dead, move);
		}

		public override void StartAction() {
			Debug.Log("Start refresh");

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