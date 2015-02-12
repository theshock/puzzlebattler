using Match.Gem;
using System.Collections;
using UnityEngine;

namespace Match.Actions {
	public class CRefreshPosition : CBase, IMoveObserver {
		public CField mIconField;

		public static IAction create() {
			return new CRefreshPosition();
		}

		public override bool Validation() {
			return mIconField.HasEmptyIcon();
		}

		public override void StartAction() {
			var move = mIconField.UpdatePositionIcons();

			if (move.IsFinished()) {
				LaunchAutoMatch();
			} else {
				move.SetObserver(this);
			}
		}

		public void OnMoveEnd () {
			LaunchAutoMatch();
		}

		public void doUpdate() {}

		private void LaunchAutoMatch () {
			var action = mActionManager.createAction(EAction.AutoMatch) as Actions.CAutoMatch;
			action.autoConfigure();
			mActionManager.AddAction(action);

			ComplateAction();
		}

		public override EEvents GetActionEvent() {
			return EEvents.RefreshPosition;
		}
	}

}