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
			foreach (CIcon icon in field.icons) {
				if (icon.IsDead()) {
					return true;
				}
			}

			return false;
		}

		private void DeadFreeFall (int c, List<CIcon> dead, CMove move) {
			int number = 0;
			int height = field.height;

			foreach (CIcon icon in dead) {
				field.SetIconAt(new CCell(height - dead.Count + number, c), icon);

				icon.SetState(EState.Idle);
				icon.gameObject.transform.position = field.GetIconCenterByCell(
					new CCell(height + number, c)
				);
				icon.SetRandomColor();
				move.AddMove( icon, field.GetIconCenterByCell(icon.cell) );

				number++;
			}
		}

		private void ColumnFreeFall (int c, CMove move) {
			List<CIcon> dead = new List<CIcon>();

			for ( int r = 0; r < field.height; r++ ) {
				CIcon current = field.GetIconAt(new CCell(r, c));

				if (current.IsDead()) {
					dead.Add(current);
					continue;
				} else if (dead.Count == 0) {
					continue; // do nothing - current now at correct place
				}

				field.SetIconAt(new CCell(r - dead.Count, c), current);

				move.AddMove( current, field.GetIconCenterByCell(current.cell) );
			}

			DeadFreeFall(c, dead, move);
		}

		public override void StartAction() {
			var move = new CMove();

			for ( int col = 0; col < field.width; col++ ) {
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