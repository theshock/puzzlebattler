using Libraries.ActionSystem;
using Match.Gem;
using System.Collections;
using UnityEngine;

namespace Match.Actions {
	public class CRefreshPosition : CBase, IMoveObserver {
		protected CField mIconField;

		public CRefreshPosition (CField field) {
			mIconField = field;
		}

		public override bool Validation() {
			return mIconField.HasEmptyIcon();
		}

		public override void StartAction() {
			var move = mIconField.UpdatePositionIcons();

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